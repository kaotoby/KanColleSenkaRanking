﻿using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanColleSenkaRanking.Models
{
    public class SenkaServerData
    {

        #region Public Declarition
        /// <summary>
        /// Last update time of the server data.
        /// </summary>
        public SenkaTimeInfo LastUpdateTime { get { return _lastUpdateTime; } }
        /// <summary>
        /// Last update time of the server data.
        /// </summary>
        public string LastUpdateTimeString { get { return _lastUpdateTime.DateTime.ToString("yyyy年M月d日 H時"); } }
        /// <summary>
        /// Thte time used to compare to current data.
        /// </summary>
        public SenkaTimeInfo CompareToTime { get { return _compareToTime; } }
        /// <summary>
        /// Server is enabled for parseing or not.
        /// </summary>
        public bool Enabled { get { return _enabled; } }
        /// <summary>
        /// The number repersent the server.
        /// </summary>
        public int ID { get { return _id; } }
        /// <summary>
        /// The server name.
        /// </summary>
        public string Name { get { return _name; } }
        /// <summary>
        /// The releative Url to for the server
        /// </summary>
        public string Url { get { return string.Format("/server/{0}", _id); } }
        #endregion

        #region Private Declarition
        private SenkaTimeInfo _lastUpdateTime;
        private SenkaTimeInfo _compareToTime;
        private bool _enabled;
        private int _id;
        private string _name;
        #endregion

        public SenkaServerData(object id, object name, object username) {
            _id = Convert.ToInt32(id);
            _name = (string)name;
            _enabled = !Convert.IsDBNull(username);
        }

        public void RefreshLastUpdateTime() {
            string _sql = "SELECT Dates.* FROM Senka" +
                @" JOIN Dates ON Senka.DateID = Dates.ID" +
                @" WHERE ServerID = " + _id.ToString() +
                @" GROUP BY Date ORDER BY Dates.ID DESC LIMIT 2";

            using (var DataBaseConnection = SenkaManager.NewSQLiteConnection())
            using (var cmd = new SQLiteCommand(_sql, DataBaseConnection)) {
                DataBaseConnection.Open();
                using (var reader = cmd.ExecuteReader()) {
                    if (reader.Read()) {
                        _lastUpdateTime = new SenkaTimeInfo(reader["ID"], reader["Date"]);
                        if (reader.Read()) {
                            _compareToTime = new SenkaTimeInfo(reader["ID"], reader["Date"]);
                        } else {
                            _compareToTime = _lastUpdateTime;
                        }
                    } else {
                        _enabled = false;
                    }
                }
            }
        }

        public void CheckForNewData() {
            DateTime now = DateTime.UtcNow.AddHours(9); //UTC+9 JST
            if (now > _lastUpdateTime.DateTime.AddHours(12)) {
                RefreshLastUpdateTime();
            }
        }

        public bool GetDateID(DateTime date, out long dateID, out long compareDateID) {
            if (date.Hour != 3 && date.Hour != 15) {
                dateID = compareDateID = 0;
                return false;
            } else {
                string _sql1 = "SELECT ID FROM Dates WHERE Date = @Date";
                string _sql2 = "SELECT MAX(DateID) FROM Senka WHERE DateID < @DateID AND ServerID = " + _id.ToString();
                try {
                    using (var DataBaseConnection = SenkaManager.NewSQLiteConnection())
                    using (var cmd = new SQLiteCommand(_sql1, DataBaseConnection)) {
                        DataBaseConnection.Open();
                        cmd.Parameters.Add(new SQLiteParameter("@Date", DbType.DateTime));
                        cmd.Parameters["@Date"].Value = date;
                        object _dateID = cmd.ExecuteScalar();
                        if (_dateID != null) {
                            dateID = Convert.ToInt64(_dateID);
                            cmd.CommandText = _sql2;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SQLiteParameter("@DateID", DbType.Int64));
                            cmd.Parameters["@DateID"].Value = dateID;
                            object _compareID = cmd.ExecuteScalar();
                            if (_compareID != null) {
                                compareDateID = Convert.ToInt64(_compareID);
                            } else {
                                throw new FormatException();
                            }
                        } else {
                            throw new FormatException();
                        }
                    }
                    return true;
                } catch (Exception) {
                    dateID = compareDateID = 0;
                    return false;
                }
            }
        }

        public IList<SenkaData> GetRankingList(int limit) {
            CheckForNewData();
            return GetRankingList(limit, _lastUpdateTime.ID, _compareToTime.ID);
        }

        public IList<SenkaData> GetRankingList(int limit, long dateID, long compareDateID) {
            List<SenkaData> latestDataset = new List<SenkaData>();
            Dictionary<long, SenkaData> compareDataset = new Dictionary<long, SenkaData>();

            //Don't need to check for new data again, due to it's benn signed already.
            string serverSQL = " WHERE DateID = @DateID AND ServerID = @ServerID";
            string latestSQL;
            if (limit == 0) {
                latestSQL = " AND (Ranking <= 100 OR Ranking = 500 OR Ranking = 990)";
            } else {
                latestSQL = " AND Ranking <= " + limit.ToString();
            }
            using (var DataBaseConnection = SenkaManager.NewSQLiteConnection())
            using (var cmd = new SQLiteCommand(SenkaManager.DefaultSQL + serverSQL + latestSQL, DataBaseConnection)) {
                DataBaseConnection.Open();
                SQLiteParameter[] paras = new SQLiteParameter[2] {
                        new SQLiteParameter("@DateID", DbType.Int64),
                        new SQLiteParameter("@ServerID", DbType.Int32)
                };
                cmd.Parameters.AddRange(paras);
                cmd.Parameters["@ServerID"].Value = _id;
                cmd.Parameters["@DateID"].Value = dateID;
                //Latest Data
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
                        data["Medals"] = reader["Medals"];
                        data["RankName"] = reader["RankName"];
                        latestDataset.Add(new SenkaData(data));
                    }
                }
                //Compare Data
                if (dateID != compareDateID) {
                    cmd.CommandText = SenkaManager.DefaultSQL + serverSQL;
                    cmd.Parameters["@DateID"].Value = compareDateID;
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
                            SenkaData senkadata = new SenkaData(data);
                            compareDataset.Add(senkadata.PlayerID, senkadata);
                        }
                    }

                    foreach (var newdata in latestDataset) {
                        long playerID = newdata.PlayerID;
                        if (compareDataset.ContainsKey(playerID)) {
                            newdata.SetDelta(compareDataset[playerID]);
                        }
                    }
                }
            }
            return latestDataset.OrderBy(d => d.Ranking).ToList();
        }
    }
}
