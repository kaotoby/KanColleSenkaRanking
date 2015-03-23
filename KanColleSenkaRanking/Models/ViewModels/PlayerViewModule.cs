﻿using KanColleSenkaRanking.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KanColleSenkaRanking.ViewModels
{
    public class PlayerViewModule
    {
        public SenkaData Player { get { return _playerData; } }
        public SenkaServerData Server { get { return _server; } }
        public IList<SenkaData> Activity { get { return _activity; } }
        public string RankingHtmlClass { get { return _rankingHtmlClass; } }
        public string JRankLable {
            get {
                return JsonConvert.SerializeObject(_rankPointChart.Lables);
            }
        }
        public string JRankPointDeltaLable {
            get {
                return JsonConvert.SerializeObject(_rankPointDeltaChart.Lables);
            }
        }
        public string JRankingData {
            get {
                return _rankingChart.ToJsonString();
            }
        }
        public string JRankPointDeltaData {
            get {
                return _rankPointDeltaChart.ToJsonString();
            }
        }
        public string JRankPointData {
            get {
                return _rankPointChart.ToJsonString();
            }
        }

        private SenkaData _playerData;
        private SenkaServerData _server;
        private IList<SenkaData> _activity;
        private string _rankingHtmlClass;
        private ChartData _rankingChart;
        private ChartData _rankPointDeltaChart;
        private ChartData _rankPointChart;
        private SenkaManager serverManager = DependencyResolver.Current.GetService<SenkaManager>();

        public PlayerViewModule(long playerID) {
            IList<SenkaData> chartDataSet = serverManager.GetPlayerDataList(playerID, out _server);
            if (_server != null) {
                IList<ChartData> charts = GetPlayerCharts(chartDataSet, _server.ID);
                _playerData = chartDataSet.Last();
                _rankingChart = charts[0];
                _rankPointDeltaChart = charts[1];
                _rankPointChart = charts[2];
                _activity = serverManager.GetPlayerActivityList(playerID, 3);

                if (_playerData.Date != _server.LastUpdateTime.DateTime) {
                    _rankingHtmlClass = " hidden";
                } else {
                    _rankingHtmlClass = "";
                }
            }
        }

        public IList<ChartData> GetPlayerCharts(IList<SenkaData> datas, int serverID) {
            IList<SenkaData> bound = serverManager.GetPlayerBoundList(serverID, datas.Last());
            Dictionary<int, Dictionary<DateTime, SenkaData>> boundDic = bound
                .GroupBy(r => r.Ranking)
                .ToDictionary(
                    groupitem => groupitem.Key,
                    value => value.ToDictionary(data => data.Date, data => data)
                );
            int upper = boundDic.Keys.Min();
            int lower = boundDic.Keys.Max();

            #region Declare
            List<ChartData> chartdata = new List<ChartData>();
            ChartData _rankPointChartData = new ChartData();
            ChartData _rankingChartData = new ChartData();
            ChartData _rankingDeltaChartData = new ChartData();
            SenkaData lastData = datas.Last();

            ChartJsonData _rankPointData = new ChartJsonData(string.Format("{0}({1}位)", lastData.PlayerName, lastData.Ranking));
            ChartJsonData _rankingaData = new ChartJsonData("順位");
            ChartJsonData _deltaAM = new ChartJsonData("3～15時");
            ChartJsonData _deltaPM = new ChartJsonData("15～27時");
            ChartJsonData _upperData = new ChartJsonData(string.Format("{0}位", upper));
            ChartJsonData _lowerData = new ChartJsonData(string.Format("{0}位", lower));

            List<string> lables = new List<string>();
            List<string> deltaLables = new List<string>();
            List<int> rankPointValues = new List<int>();
            List<int> rankingValues = new List<int>();
            List<int> deltaAMValues = new List<int>();
            List<int> deltaPMValues = new List<int>();
            List<int> upperValues = new List<int>();
            List<int> lowerValues = new List<int>();
            #endregion

            Dictionary<DateTime, SenkaData> dic = datas.ToDictionary(d => d.Date, d => d);
            DateTime end = boundDic[lower].Keys.Max();
            DateTime last = boundDic[lower].Keys.Min();
            DateTime date = new DateTime(last.Year, last.Month, last.Day, last.Hour, 0, 0);
            if (last.Hour == 3 && last.Day != 1) {
                deltaAMValues.Add(ChartData.NONE);
            }

            while (date <= end) {
                if (dic.ContainsKey(date)) {
                    SenkaData item = dic[date];
                    if (upper != lower) upperValues.Add(boundDic[upper][date].RankPoint);
                    lowerValues.Add(boundDic[lower][date].RankPoint);
                    rankPointValues.Add(item.RankPoint);
                    rankingValues.Add(item.Ranking);
                    //Delta Data
                    if (date.Day == 1 && date.Hour == 15) {
                        deltaAMValues.Add(item.RankPoint);
                    } else if ((date - last).Hours == 12 && dic.ContainsKey(last)) {
                        int delta = item.RankPoint - dic[last].RankPoint;
                        if (date.Hour == 3) deltaPMValues.Add(delta);
                        else deltaAMValues.Add(delta);
                    } else {
                        if (date.Hour == 3) deltaPMValues.Add(ChartData.NONE);
                        else deltaAMValues.Add(ChartData.NONE);
                    }
                    last = date;
                } else {
                    if (boundDic[lower].ContainsKey(date)) {
                        if (upper != lower) upperValues.Add(boundDic[upper][date].RankPoint);
                        lowerValues.Add(boundDic[lower][date].RankPoint);
                    } else {
                        if (upper != lower) upperValues.Add(ChartData.NONE);
                        lowerValues.Add(ChartData.NONE);
                    }
                    rankPointValues.Add(ChartData.NONE);
                    rankingValues.Add(ChartData.NONE);
                    if (date.Hour == 3) deltaPMValues.Add(ChartData.NONE);
                    else deltaAMValues.Add(ChartData.NONE);
                }
                //Lables
                if (date.Hour == 3) {
                    lables.Add(date.Day.ToString());
                } else {
                    deltaLables.Add(date.Day.ToString());
                    lables.Add("");
                }
                date = date.AddHours(12);
            }
            if (end.Hour == 15) {
                deltaPMValues.Add(ChartData.NONE);
            }
            if (deltaAMValues[0] == ChartData.NONE && deltaPMValues[0] == ChartData.NONE) {
                deltaAMValues.RemoveAt(0);
                deltaPMValues.RemoveAt(0);
            }
            var amd = deltaAMValues.Distinct();
            var pmd = deltaPMValues.Distinct();
            if (amd.Count() == 1 && pmd.Count() == 1 && amd.First() == ChartData.NONE && pmd.First() == ChartData.NONE) {
                deltaAMValues.Clear();
                deltaPMValues.Clear();
            }

            _upperData.value = ValueConveter(upperValues);
            _lowerData.value = ValueConveter(lowerValues);
            _rankPointData.value = ValueConveter(rankPointValues);
            _rankingaData.value = ValueConveter(rankingValues);
            _deltaAM.value = ValueConveter(deltaAMValues);
            _deltaPM.value = ValueConveter(deltaPMValues);

            ChartJsonData[] _rankPointChartJsonData;
            if (upperValues.Count == 0) {
                _rankPointChartJsonData = new ChartJsonData[2] { _rankPointData, _lowerData };
            } else {
                _rankPointChartJsonData = new ChartJsonData[3] { _rankPointData, _lowerData, _upperData };
            }

            _rankPointChartData.SetData(_rankPointChartJsonData, lables.ToArray());
            _rankingChartData.SetData(new ChartJsonData[1] { _rankingaData }, lables.ToArray());
            _rankingDeltaChartData.SetData(new ChartJsonData[2] { _deltaAM, _deltaPM }, deltaLables.ToArray());

            chartdata.Add(_rankingChartData);
            chartdata.Add(_rankingDeltaChartData);
            chartdata.Add(_rankPointChartData);
            return chartdata;
        }

        private double[] ValueConveter(List<int> values) {
            if (values.Count == 0) {
                return new double[0];
            } else if (values.Count == 1 && values[0] == ChartData.NONE) {
                return new double[1] { 0 };
            } else {
                double min = values.Min();
                return values.Select(v => v == ChartData.NONE ? min + 0.1 : v).ToArray();
            }
        }
    }
}