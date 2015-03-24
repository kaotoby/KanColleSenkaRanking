using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanColleSenkaService.Module;
using System.Data.SQLite;
using System.Net;
using System.Collections.Specialized;
using log4net;
using System.Threading;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace KanColleSenkaService
{
    public class KanColleSenkaManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(KanColleSenkaManager).FullName);

        public static SQLiteConnection NewSQLiteConnection() {
            return new SQLiteConnection(@"Data Source=|DataDirectory|kansenka.db;Version=3;New=False;Compress=True;");
        }


        public IList<ServerData> GetServerData() {
            List<ServerData> serverdata = new List<ServerData>();
            string _sql = @"SELECT * FROM Servers";
            using (var DataBaseConnection = NewSQLiteConnection())
            using (var cmd = new SQLiteCommand(_sql, DataBaseConnection)) {
                DataBaseConnection.Open();
                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        serverdata.Add(new ServerData(
                            reader["ID"],
                            reader["Name"],
                            reader["UserName"],
                            reader["Password"]));
                    }
                }
            }
            foreach (var server in serverdata) {
                server.GetToken();
            }
            return serverdata;
        }

        public void ProcessServerData(ServerData serverdata) {
            log.Debug(string.Format("[ServerID {0}] Server update started", serverdata.ID));
            serverdata.InitializeUpdate();

            HttpHelper helper = new HttpHelper();
            Random rand = new Random();
            Dictionary<string, string> postDic = new Dictionary<string, string>();
            Dictionary<string, string> customHeader = new Dictionary<string, string>();
            string referer = serverdata.SwfPath + "/[[DYNAMIC]]/1";
            customHeader["Pragma"] = "no-cache";
            customHeader["x-flash-version"] = "16,0,0,305";
            int currentRetryPage = 0, currentRetryCount = 0;

#if DEBUG
            for (int i = 1; i <= 3; i++) {
#else
            for (int i = 1; i < 100; i++) {
#endif
                string jsonResult = "";
                postDic["api_pageno"] = i.ToString();
                postDic["api_verno"] = "1";
                postDic["api_token"] = serverdata.ApiToken;
                helper.CTRHttp(serverdata.FullPath, referer, postDic, customHeader, ref jsonResult);

                if (jsonResult == "" || BatchParse(jsonResult, serverdata, i)) {
                    Thread.Sleep(rand.Next(700, 2000));
                } else {
                    if (i != currentRetryPage) {
                        currentRetryPage = i;
                        currentRetryCount = 0;
                    } else if (currentRetryCount == 2) {
                        string errMsg = string.Format("[ServerID {0}] Page {1}, request failed!", serverdata.ID, i);
                        log.Error(errMsg);
                        throw new WebException(errMsg);
                    }
                    i--;
                    Thread.Sleep(1000 * 3 * (int)Math.Pow(10, currentRetryCount++));
                }
            }

            DateTime newtime = new DateTime(serverdata.Date.Year, serverdata.Date.Month, serverdata.Date.Day,
                serverdata.Date.Hour, 2, 0).AddHours(12);
            serverdata.ErrorrCount = 0;
            serverdata.NextUpdateTime = newtime;
            serverdata.SaveToDataBase();
            serverdata.SaveExpToDataBase();
            log.Info(string.Format("[ServerID {0}] Server update finished", serverdata.ID));
        }

        public bool BatchParse(string jsonString, ServerData serverdata, int page) {
            JObject jsonData;
            try {
                jsonData = JObject.Parse(jsonString.Replace("svdata=", ""));
                JValue jsonResult = (JValue)jsonData["api_result"];
                if (Convert.ToInt32(jsonResult.Value) == 1) {
                    IList<JToken> results = jsonData["api_data"]["api_list"].Children().ToList();
                    foreach (JToken result in results) {
                        ApiSenkaResult apiResult = JsonConvert.DeserializeObject<ApiSenkaResult>(result.ToString());
                        serverdata.AddData(new SenkaData(apiResult));
                    }
                } else {
                    throw new WebException(jsonString);
                }
            } catch (Exception ex) {
                log.Warn(string.Format("[ServerID {0}] Page {1} Pharing ERROR!", serverdata.ID, page), ex);
                return false;
            }
            return true;
        }
    }
}
