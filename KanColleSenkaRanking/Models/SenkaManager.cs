﻿using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Web;

namespace KanColleSenkaRanking.Models
{
    public class SenkaManager
    {
        public IDictionary<int, SenkaServerData> Servers { get { return _servers; } }
        public const string DefaultSQL = "SELECT Dates.Date, Senka.*, RankType.RankName FROM Senka" +
                @" JOIN RankType ON Senka.RankType = RankType.ID" +
                @" JOIN Dates ON Senka.DateID = Dates.ID";

        private Dictionary<int, SenkaServerData> _servers;
        private static readonly ILog log = LogManager.GetLogger(typeof(SenkaManager).FullName);

        public static SQLiteConnection NewSQLiteConnection() {
            return new SQLiteConnection(@"Data Source=|DataDirectory|kansenka.db;Version=3;New=False;Compress=True;");
        }

        public SenkaManager() {
            _servers = new Dictionary<int, SenkaServerData>();
            InitializeServerData();
        }

        private void InitializeServerData() {
            List<SenkaServerData> serverdata = new List<SenkaServerData>();

            string _sql = @"SELECT * FROM Servers";
            using (var DataBaseConnection = NewSQLiteConnection())
            using (var cmd = new SQLiteCommand(_sql, DataBaseConnection)) {
                DataBaseConnection.Open();
                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        serverdata.Add(new SenkaServerData(
                            reader["ID"],
                            reader["Name"],
                            reader["UserName"]
                            ));
                    }
                }
            }
            foreach (var server in serverdata) {
                _servers[server.ID] = server;
                if (server.Enabled) server.RefreshLastUpdateTime();
            }
        }

        public IList<SenkaData> GetPlayerActivityList(long playerID, int limit) {
            List<SenkaData> dataset = new List<SenkaData>();

            string _sql = "SELECT *, MIN(DateID) FROM (" + DefaultSQL + " WHERE PlayerID = @PlayerID)" +
                @" GROUP BY Comment, Level ORDER BY DateID DESC LIMIT " + limit.ToString();

            using (var DataBaseConnection = NewSQLiteConnection())
            using (var cmd = new SQLiteCommand(_sql, DataBaseConnection)) {
                DataBaseConnection.Open();
                SQLiteParameter para = new SQLiteParameter("@PlayerID", DbType.Int64);
                cmd.Parameters.Add(para);
                cmd.Parameters["@PlayerID"].Value = playerID;

                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        dataset.Add(new SenkaData(reader));
                    }
                }
            }
            return dataset;
        }

        public IList<SenkaData> GetPlayerDataList(long playerID, out SenkaServerData serverData) {
            DateTime now = DateTime.UtcNow.AddHours(9).AddHours(-12); //UTC+9 - 12hr
            DateTime start = new DateTime(now.Year, now.Month, 1);
            return GetPlayerDataList(playerID, start, out serverData);
        }

        public IList<SenkaData> GetPlayerDataList(long playerID, DateTime start, out SenkaServerData serverData) {
            DateTime end = start.AddMonths(1).AddMinutes(-1);
            List<SenkaData> dataset = new List<SenkaData>();
            serverData = null;

            string playerSQL = " WHERE PlayerID = @PlayerID AND Date > @DateStart AND Date < @DateEnd";
            using (var DataBaseConnection = NewSQLiteConnection())
            using (var cmd = new SQLiteCommand(DefaultSQL + playerSQL, DataBaseConnection)) {
                DataBaseConnection.Open();
                SQLiteParameter[] paras = new SQLiteParameter[3] {
                    new SQLiteParameter("@PlayerID", DbType.Int64),
                    new SQLiteParameter("@DateStart", DbType.DateTime),
                    new SQLiteParameter("@DateEnd", DbType.DateTime)
                };
                cmd.Parameters.AddRange(paras);
                cmd.Parameters["@PlayerID"].Value = playerID;
                cmd.Parameters["@DateStart"].Value = start;
                cmd.Parameters["@DateEnd"].Value = end;

                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        dataset.Add(new SenkaData(reader));
                        if (serverData == null) {
                            serverData = _servers[Convert.ToInt32(reader["ServerID"])];
                        }
                    }
                }
            }
            return dataset.OrderBy(d => d.Date).ToList();
        }

        public IList<SenkaData> GetPlayerBoundList(int serverID, SenkaData lastData) {
            DateTime start = new DateTime(lastData.Date.DateTime.Year, lastData.Date.DateTime.Month, 1);
            List<SenkaData> dataset = new List<SenkaData>();
            int upper, lower;
            if (lastData.Ranking == 1) {
                upper = 0;
                lower = 5;
            } else if (lastData.Ranking <= 5) {
                upper = 1;
                lower = 5;
            } else if (lastData.Ranking <= 20) {
                upper = 5;
                lower = 20;
            } else if (lastData.Ranking <= 100) {
                upper = 20;
                lower = 100;
            } else if (lastData.Ranking <= 500) {
                upper = 100;
                lower = 500;
            } else {
                upper = 500;
                lower = 990;
            }

            string playerSQL = " WHERE (Ranking = @RankingUpper OR Ranking = @RankingLower)" +
                @" AND Date > @DateStart AND Date <= @DateEnd" +
                @" AND ServerID = @ServerID";
            using (var DataBaseConnection = NewSQLiteConnection())
            using (var cmd = new SQLiteCommand(DefaultSQL + playerSQL, DataBaseConnection)) {
                DataBaseConnection.Open();
                SQLiteParameter[] paras = new SQLiteParameter[5] {
                    new SQLiteParameter("@RankingUpper", DbType.Int32),
                    new SQLiteParameter("@RankingLower", DbType.Int32),
                    new SQLiteParameter("@DateStart", DbType.DateTime),
                    new SQLiteParameter("@DateEnd", DbType.DateTime),
                    new SQLiteParameter("@ServerID", DbType.Int32)
                };
                cmd.Parameters.AddRange(paras);
                cmd.Parameters["@RankingUpper"].Value = upper;
                cmd.Parameters["@RankingLower"].Value = lower;
                cmd.Parameters["@DateStart"].Value = start;
                cmd.Parameters["@DateEnd"].Value = lastData.Date.DateTime;
                cmd.Parameters["@ServerID"].Value = serverID;

                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        dataset.Add(new SenkaData(reader));
                    }
                }
            }
            return dataset;
        }

        public IList<Tuple<SenkaData, string>> GetAllServerRanking(int limit) {
            List<Tuple<SenkaData, string>> dataset = new List<Tuple<SenkaData, string>>();

            string _sql = "SELECT Servers.NickName, Senka.*, Dates.Date, RankType.RankName FROM Senka" +
                @" JOIN RankType ON Senka.RankType = RankType.ID" +
                @" JOIN Dates ON Senka.DateID = Dates.ID" +
                @" JOIN Servers ON Senka.ServerID = Servers.ID" +
                @" WHERE Senka.DateID = (SELECT MIN(LastUpdate) FROM Servers)" + 
                @" ORDER BY RankPoint DESC LIMIT " + limit.ToString();
            using (var DataBaseConnection = NewSQLiteConnection())
            using (var cmd = new SQLiteCommand(_sql, DataBaseConnection)) {
                DataBaseConnection.Open();
                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        SenkaData data = new SenkaData(reader);
                        string server = Convert.ToString(reader["NickName"]);
                        Tuple<SenkaData, string> tuple = new Tuple<SenkaData, string>(data, server);
                        dataset.Add(tuple);
                    }
                }
            }
            return dataset;
        }
    }
}