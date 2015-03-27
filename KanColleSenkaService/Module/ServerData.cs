using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanColleSenkaService.Module
{
    public class ServerData
    {

        #region Public Declarition
        /// <summary>
        /// Server is enabled for parseing or not.
        /// </summary>
        public bool Enabled { get { return _ip != null; } }
        /// <summary>
        /// The time that next update withh be performed.
        /// </summary>
        public DateTime NextUpdateTime { get; set; }
        /// <summary>
        /// The time that data collected.
        /// </summary>
        public DateTime Date { get { return _date; } }
        /// <summary>
        /// The number repersent the server.
        /// </summary>
        public int ID { get { return _id; } }
        /// <summary>
        /// The server name.
        /// </summary>
        public string Name { get { return _name; } }
        /// <summary>
        /// The server IP address.
        /// </summary>
        public string IP { get { return _ip; } }
        /// <summary>
        /// The API Token
        /// </summary>
        public string ApiToken { get { return _apiToken; } }
        /// <summary>
        /// The path to the senka list.
        /// </summary>
        public const string Path = "/kcsapi/api_req_ranking/getlist";
        /// <summary>
        /// The full path to the senka list.
        /// </summary>
        public string FullPath {
            get {
                return string.Format("http://{0}{1}", _ip, Path);
            }
        }
        /// <summary>
        /// The full path to the main swf file.
        /// </summary>
        public string SwfPath {
            get {
                return string.Format("http://{0}/kcs/mainD2.swf?api_token={1}&api_starttime={2}", _ip, _apiToken, _apiStartTime);
            }
        }
        /// <summary>
        /// The errorr count when process the update.
        /// </summary>
        public int ErrorrCount {
            get {
                return _errorCount;
            }
            set {
                if (value > 10) {
                    _errorCount = 0;
                    log.Fatal(string.Format("[ServerID {0}] Update disabled due to too many fail!", _id));
                    _ip = null;
                } else {
                    if (value != 0) GetToken();
                    _errorCount = value;
                }
            }
        }
        /// <summary>
        /// The log file path to be saved.
        /// </summary>
        public string LogPath {
            get {
                return string.Format(@"{0}[S{1:D2}]{2:yyMMddHH}.log", _logpdir, _id, _date);
            }
        }
        #endregion

        #region Private Declarition
        private long _dateID;
        private DateTime _date;
        private int _id;
        private string _name;
        private string _ip;
        private string _apiToken;
        private string _apiStartTime;
        private string _username;
        private string _password;
        private int _errorCount = 0;
        private List<SenkaData> _dataSet = new List<SenkaData>();
        private string _logpdir = AppDomain.CurrentDomain.BaseDirectory + @"SenkaLog\";
        private static readonly ILog log = LogManager.GetLogger(typeof(ServerData).FullName);
        #endregion

        public ServerData(object id, object name, object username, object password) {
            _id = Convert.ToInt32(id);
            _name = (string)name;
            _username = Convert.IsDBNull(username) ? "" : (string)username;
            _password = Convert.IsDBNull(password) ? "" : (string)password;
            NextUpdateTime = DateTime.Now;
        }

        public void AddData(SenkaData data) {
            _dataSet.Add(data);
        }

        public void InitializeUpdate(DateTime date, long dateID) {
            _dataSet.Clear();
            _date = date;
            _dateID = dateID;
            if (File.Exists(this.LogPath)) {
                File.Delete(this.LogPath);
            }
        }

        public void GetToken() {
            //Login to DMM
            if (_username == "" || _password == "") return;

            DMMLoginHelper helper = new DMMLoginHelper(_username, _password, _id);
#if DEBUG
            if (_id == 19) {
                helper.Process(out _ip, out _apiToken, out _apiStartTime);
            }
#else
            try {
                helper.Process(out _ip, out _apiToken, out _apiStartTime);
            } catch (Exception ex) {
                _ip = null;
                log.Error(string.Format("[ServerID {0}] Login fail!", _id), ex);
            }
#endif
        }

        public void CheckDataBase() {
            int dataCount = 0;
            string _sqlSelect = @"SELECT COUNT(*) FROM Senka WHERE DateID = @DateID AND ServerID = @ServerID";
            string _sqlDelete = @"DELETE FROM Senka WHERE DateID = @DateID AND ServerID = @ServerID";
            using (var DataBaseConnection = KanColleSenkaManager.NewSQLiteConnection()) {
                DataBaseConnection.Open();
                using (var transaction = DataBaseConnection.BeginTransaction()) {
                    using (var cmd = DataBaseConnection.CreateCommand()) {
                        SQLiteParameter dateID = new SQLiteParameter("@DateID", DbType.Int64);
                        SQLiteParameter serverID = new SQLiteParameter("@ServerID", DbType.Int32);
                        dateID.Value = _dateID;
                        serverID.Value = _id;
                        cmd.Parameters.Add(dateID);
                        cmd.Parameters.Add(serverID);

                        cmd.CommandText = _sqlSelect;
                        dataCount = Convert.ToInt32(cmd.ExecuteScalar());

                        if (dataCount > 0) {
                            cmd.CommandText = _sqlDelete;
                            cmd.ExecuteNonQuery();
                            log.Info(string.Format("[ServerID {0}] {1} datas were deleted and have been re-requested.", _id, dataCount));
                        }
                    }
                    transaction.Commit();
                }
            }
        }

        public void SaveToDataBase() {
            int count = _dataSet.Count;
            if (count != 990) {
                log.Warn(string.Format("[ServerID {0}] Data count is {1}", _id, count));
            }

            CheckDataBase();
            string _sql = @"INSERT INTO Senka VALUES " +
                @"(@DateID, @ServerID, @Ranking, @Level, @PlayerName, @PlayerID, @Comment, @RankPoint, @RankType, @Medals)";
            using (var DataBaseConnection = KanColleSenkaManager.NewSQLiteConnection()) {
                DataBaseConnection.Open();
                using (var transaction = DataBaseConnection.BeginTransaction()) {
                    using (var cmd = new SQLiteCommand(_sql, DataBaseConnection)) {
                        SQLiteParameter[] paras = new SQLiteParameter[10]{
                            new SQLiteParameter("@DateID", DbType.Int64),
                            new SQLiteParameter("@ServerID", DbType.Int32),
                            new SQLiteParameter("@Ranking", DbType.Int32),
                            new SQLiteParameter("@Level", DbType.Int32),
                            new SQLiteParameter("@PlayerName", DbType.String),
                            new SQLiteParameter("@PlayerID", DbType.Int64),
                            new SQLiteParameter("@Comment", DbType.String),
                            new SQLiteParameter("@RankPoint", DbType.Int32),
                            new SQLiteParameter("@RankType", DbType.Int32),
                            new SQLiteParameter("@Medals", DbType.Int32)
                        };
                        cmd.Parameters.AddRange(paras);

                        foreach (var data in _dataSet) {
                            cmd.Parameters["@DateID"].Value = _dateID;
                            cmd.Parameters["@ServerID"].Value = _id;
                            cmd.Parameters["@Ranking"].Value = data.Ranking;
                            cmd.Parameters["@Level"].Value = data.Level;
                            cmd.Parameters["@PlayerName"].Value = data.PlayerName;
                            cmd.Parameters["@PlayerID"].Value = data.PlayerID;
                            cmd.Parameters["@Comment"].Value = data.Comment;
                            cmd.Parameters["@RankPoint"].Value = data.RankPoint;
                            cmd.Parameters["@RankType"].Value = data.RankType;
                            cmd.Parameters["@Medals"].Value = data.Medals;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
            }
        }

        public void SaveExpToDataBase() {
            DateTime current = DateTime.UtcNow.AddHours(6);
            DateTime lastday = new DateTime(current.Year, current.Month, 1, 15, 0, 0).AddMonths(1).AddDays(-1);

            if (lastday == _date) {
                string _sql = @"INSERT INTO Exp VALUES (@DateID, @PlayerID, @Exp)";
                using (var DataBaseConnection = KanColleSenkaManager.NewSQLiteConnection()) {
                    DataBaseConnection.Open();
                    using (var transaction = DataBaseConnection.BeginTransaction()) {
                        using (var cmd = new SQLiteCommand(_sql, DataBaseConnection)) {
                            SQLiteParameter[] paras = new SQLiteParameter[3]{
                            new SQLiteParameter("@DateID", DbType.Int64),
                            new SQLiteParameter("@PlayerID", DbType.Int64),
                            new SQLiteParameter("@Exp", DbType.Int32)
                        };
                            cmd.Parameters.AddRange(paras);

                            foreach (var data in _dataSet) {
                                cmd.Parameters["@DateID"].Value = _dateID;
                                cmd.Parameters["@PlayerID"].Value = data.PlayerID;
                                cmd.Parameters["@Exp"].Value = data.Exp;
                                cmd.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                    }
                }
            }
        }
    }
}
