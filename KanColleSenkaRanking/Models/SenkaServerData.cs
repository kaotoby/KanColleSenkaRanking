using log4net;
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
    }
}
