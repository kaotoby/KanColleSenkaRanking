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
        public IList<SenkaData> GetFullPlayerList(int serverID) {
            List<SenkaData> playerList = new List<SenkaData>();
            SenkaServerData server = _servers[serverID];
            if (!server.Enabled) return playerList;

            string serverSQL = " WHERE ServerID = @ServerID GROUP BY PlayerID";
            using (var DataBaseConnection = NewSQLiteConnection())
            using (var cmd = new SQLiteCommand(DefaultSQL + serverSQL, DataBaseConnection)) {
                DataBaseConnection.Open();
                SQLiteParameter[] paras = new SQLiteParameter[1] {
                        new SQLiteParameter("@ServerID", DbType.Int32)
                };
                cmd.Parameters.AddRange(paras);
                cmd.Parameters["@ServerID"].Value = server.ID;
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
                        playerList.Add(new SenkaData(data));
                    }
                }
            }
            return playerList;
        }
    }
}