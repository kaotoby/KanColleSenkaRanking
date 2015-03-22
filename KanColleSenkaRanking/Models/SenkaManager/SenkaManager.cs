using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Web;

namespace KanColleSenkaRanking.Models
{
    public partial class SenkaManager
    {
        public IDictionary<int, SenkaServerData> Servers { get { return _servers; } }

        private Dictionary<int, SenkaServerData> _servers;
        private static readonly ILog log = LogManager.GetLogger(typeof(SenkaManager).FullName);
        private const string _defaultSQL = "SELECT Dates.Date, Senka.*, RankType.RankName FROM Senka" +
                @" JOIN RankType ON Senka.RankType = RankType.ID" +
                @" JOIN Dates ON Senka.DateID = Dates.ID";

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

        public IList<SenkaData> GetPlayerBoundList(int serverID, SenkaData lastData) {
            DateTime start = new DateTime(lastData.Date.Year,lastData.Date.Month,1);
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
                lower = 1000;
            }

            string playerSQL = " WHERE (Ranking = @RankingUpper OR Ranking = @RankingLower)" +
                @" AND Date > @DateStart AND Date <= @DateEnd" +
                @" AND ServerID = @ServerID";
            using (var DataBaseConnection = NewSQLiteConnection())
            using (var cmd = new SQLiteCommand(_defaultSQL + playerSQL, DataBaseConnection)) {
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
                cmd.Parameters["@DateEnd"].Value = lastData.Date;
                cmd.Parameters["@ServerID"].Value = serverID;

                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        Dictionary<string, object> data = new Dictionary<string, object>();
                        data["Date"] = reader["Date"];
                        data["Ranking"] = reader["Ranking"];
                        data["Level"] = reader["Level"];
                        data["PlayerName"] = reader["PlayerName"];
                        data["PlayerID"] = reader["PlayerID"];
                        data["Comment"] = reader["Comment"];
                        data["RankPoint"] = reader["RankPoint"];
                        data["RankName"] = reader["RankName"];
                        data["Medals"] = reader["Medals"];
                        dataset.Add(new SenkaData(data));
                    }
                }
            }
            return dataset;
        }

    }
}