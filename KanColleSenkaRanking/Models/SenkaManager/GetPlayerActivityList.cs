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