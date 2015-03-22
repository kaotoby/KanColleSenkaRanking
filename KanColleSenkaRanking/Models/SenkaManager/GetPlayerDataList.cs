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
        public IList<SenkaData> GetPlayerDataList(long playerID, out SenkaServerData serverData) {
            DateTime now = DateTime.UtcNow.AddHours(-6);
            DateTime start = new DateTime(now.Year, now.Month, 1);
            return GetPlayerDataList(playerID, start, out serverData);
        }

        public IList<SenkaData> GetPlayerDataList(long playerID, DateTime start, out SenkaServerData serverData) {
            DateTime end = start.AddMonths(1).AddMinutes(-1);
            List<SenkaData> dataset = new List<SenkaData>();
            serverData = null;

            string playerSQL = " WHERE PlayerID = @PlayerID AND Date > @DateStart AND Date < @DateEnd";
            using (var DataBaseConnection = NewSQLiteConnection())
            using (var cmd = new SQLiteCommand(_defaultSQL + playerSQL, DataBaseConnection)) {
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
                        serverData = _servers[Convert.ToInt32(reader["ServerID"])];
                        dataset.Add(new SenkaData(data));
                    }
                }
            }
            return dataset.OrderBy(d => d.Date).ToList();
        }
    }
}