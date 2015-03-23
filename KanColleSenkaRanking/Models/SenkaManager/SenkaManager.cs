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
    }
}