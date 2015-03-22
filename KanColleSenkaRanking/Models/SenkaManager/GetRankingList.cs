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
        public IList<SenkaData> GetRankingList(int serverID, int limit) {
            List<SenkaData> latestDataset = new List<SenkaData>();
            Dictionary<long, SenkaData> compareDataset = new Dictionary<long, SenkaData>();
            SenkaServerData server = _servers[serverID];
            if (!server.Enabled) return latestDataset;

            string serverSQL = " WHERE DateID = @DateID AND ServerID = @ServerID";
            string latestSQL = " AND Ranking <= @Ranking";
            using (var DataBaseConnection = NewSQLiteConnection())
            using (var cmd = new SQLiteCommand(_defaultSQL + serverSQL + latestSQL, DataBaseConnection)) {
                DataBaseConnection.Open();
                SQLiteParameter[] paras = new SQLiteParameter[3] {
                        new SQLiteParameter("@DateID", DbType.Int64),
                        new SQLiteParameter("@ServerID", DbType.Int32),
                        new SQLiteParameter("@Ranking", DbType.Int32)
                };
                cmd.Parameters.AddRange(paras);
                cmd.Parameters["@ServerID"].Value = server.ID;
                cmd.Parameters["@DateID"].Value = server.LastUpdateTime.ID;
                cmd.Parameters["@Ranking"].Value = limit;
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
                if (server.LastUpdateTime != server.CompareToTime) {
                    cmd.CommandText = _defaultSQL + serverSQL;
                    cmd.Parameters["@DateID"].Value = server.CompareToTime.ID;
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

        public IList<SenkaData> GetDefaultRankingList(int serverID) {
            SenkaServerData server = _servers[serverID];
            if (server.Enabled) {
                return GetDefaultRankingList(serverID, server.LastUpdateTime.ID, server.CompareToTime.ID);
            } else {
                return new List<SenkaData>();
            }
        }

        public IList<SenkaData> GetDefaultRankingList(int serverID, long dateID, long compareDateID) {
            List<SenkaData> latestDataset = new List<SenkaData>();
            Dictionary<long, SenkaData> compareDataset = new Dictionary<long, SenkaData>();
            SenkaServerData server = _servers[serverID];
            if (!server.Enabled) return latestDataset;

            string serverSQL = " WHERE DateID = @DateID AND ServerID = @ServerID";
            string latestSQL = " AND (Ranking <= 100 OR Ranking = 500 OR Ranking = 1000)";
            using (var DataBaseConnection = NewSQLiteConnection())
            using (var cmd = new SQLiteCommand(_defaultSQL + serverSQL + latestSQL, DataBaseConnection)) {
                DataBaseConnection.Open();
                SQLiteParameter[] paras = new SQLiteParameter[2] {
                        new SQLiteParameter("@DateID", DbType.Int64),
                        new SQLiteParameter("@ServerID", DbType.Int32)
                };
                cmd.Parameters.AddRange(paras);
                cmd.Parameters["@ServerID"].Value = server.ID;
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
                if (server.LastUpdateTime != server.CompareToTime) {
                    cmd.CommandText = _defaultSQL + serverSQL;
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