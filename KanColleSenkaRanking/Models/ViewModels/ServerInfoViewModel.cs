using KanColleSenkaRanking.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KanColleSenkaRanking.ViewModels
{
    public class ServerInfoViewModel
    {
        public SenkaServerData Server { get { return _server; } }
        public string JRankPointLable {
            get {
                return JsonConvert.SerializeObject(_rankPointChart.Lables);
            }
        }
        public string JRankPointDeltaAvgLable {
            get {
                return JsonConvert.SerializeObject(_rankPointDeltaAvgChart.Lables);
            }
        }
        public string JRankPointData {
            get {
                return _rankPointChart.ToJsonString();
            }
        }
        public string JRankPointDeltaAvgData {
            get {
                return _rankPointDeltaAvgChart.ToJsonString();
            }
        }
        public string JTopPlayerName {
            get {
                return JsonConvert.SerializeObject(_topPlayerName.ToArray());
            }
        }

        private SenkaServerData _server;
        private ChartData _rankPointChart;
        private ChartData _rankPointDeltaAvgChart;
        private List<string> _topPlayerName;
        private SenkaManager serverManager = DependencyResolver.Current.GetService<SenkaManager>();

        public ServerInfoViewModel(int serverID) {
            if (serverManager.Servers.Keys.Contains(serverID)) {
                _server = serverManager.Servers[serverID];
                if (_server.Enabled) {
                    _server.CheckForNewData();
                    CreateServerCharts(_server.GetServerBoundDic());
                }
            } else {
                throw new HttpException(404, "Not found");
            }
        }

        private void CreateServerCharts(IDictionary<int, Dictionary<DateTime, SenkaData>> dic) {
            #region Declare
            List<string> lables = new List<string>();
            _topPlayerName = new List<string>();
            _rankPointChart = new ChartData();
            _rankPointDeltaAvgChart = new ChartData();
            ChartJsonData[] _rankPoint = new ChartJsonData[dic.Count];
            ChartJsonData _avgAM = new ChartJsonData("3～15時");
            ChartJsonData _avgPM = new ChartJsonData("15～27時");

            DateTime last = dic[1].First().Key;
            DateTime end = dic[1].Last().Key;
            DateTime date = last;
            Dictionary<int, List<int>> rankPointValue = new Dictionary<int, List<int>>();
            Dictionary<int, List<int>[]> rankDeltaValue = new Dictionary<int, List<int>[]>();
            foreach (var key in dic.Keys) {
                rankPointValue[key] = new List<int>();
                rankDeltaValue[key] = new List<int>[] {
                    new List<int>(),
                    new List<int>()
                };
            }
            #endregion

            while (date <= end) {
                if (dic[1].ContainsKey(date)) {
                    foreach (var key in dic.Keys) {
                        rankPointValue[key].Add(dic[key][date].RankPoint);
                    }
                    _topPlayerName.Add(dic[1][date].PlayerName);
                    if ((date - last).Hours == 12) {
                        foreach (var key in dic.Keys) {
                            int delta = dic[key][date].RankPoint - dic[key][last].RankPoint;
                            if (date.Hour == 3) rankDeltaValue[key][1].Add(delta);
                            else rankDeltaValue[key][0].Add(delta);
                        }
                    }
                    last = date;
                } else {
                    foreach (var key in dic.Keys) {
                        rankPointValue[key].Add(ChartData.NONE);
                    }
                    _topPlayerName.Add("");
                }
                //Lables
                if (date.Hour == 3) {
                    lables.Add(date.Day.ToString());
                } else {
                    lables.Add("");
                }
                date = date.AddHours(12);
            }
            for (int i = 0; i < dic.Count; i++) {
                int key = dic.ElementAt(i).Key;
                _rankPoint[i] = new ChartJsonData(key.ToString() + "位");
                _rankPoint[i].SetValue(rankPointValue[key]);
            }
            _rankPointChart.SetData(_rankPoint, lables.ToArray());

            _avgAM.value = rankDeltaValue.Select(d => Math.Round(d.Value[0].Average(), 1)).ToArray();
            _avgPM.value = rankDeltaValue.Select(d => Math.Round(d.Value[1].Average(), 1)).ToArray();
            var avgLabel = dic.Select(d=>d.Key.ToString() + "位").ToArray();

            _rankPointDeltaAvgChart.SetData(new ChartJsonData[2] { _avgAM, _avgPM }, avgLabel);
        }
    }
}