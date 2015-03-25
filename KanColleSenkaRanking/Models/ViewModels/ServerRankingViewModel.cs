using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KanColleSenkaRanking.Models;
using System.Web.Mvc;
using System.Globalization;

namespace KanColleSenkaRanking.ViewModels
{
    public class ServerRankingViewModel
    {
        public bool IsDefaultListing { get { return _isDefaultListing; } }
        public SenkaServerData Server { get { return _server; } }
        public IList<SenkaData> RankingDataSet { get { return _rankingDataSet; } }
        public HtmlString State { get { return _state; } }
        public string Description { get { return _description; } }

        private bool _isDefaultListing;
        private SenkaServerData _server;
        private IList<SenkaData> _rankingDataSet;
        private HtmlString _state;
        private SenkaManager serverManager = DependencyResolver.Current.GetService<SenkaManager>();
        private string _description;

        public ServerRankingViewModel(int serverID, int limit, string date) {
            if (serverManager.Servers.Keys.Contains(serverID)) {
                _server = serverManager.Servers[serverID];
                if (_server.Enabled) {
                    DateTime d;
                    if (date == null) {
                        _server.CheckForNewData();
                        d = _server.LastUpdateTime.DateTime;
                        _rankingDataSet = _server.GetRankingList(limit);
                    } else {
                        long dateID, compareDateID;
                        if (DateTime.TryParseExact(date, "yyMMddHH", null, DateTimeStyles.None, out d) &&
                            _server.GetDateID(d, out dateID, out compareDateID)) {

                            _rankingDataSet = _server.GetRankingList(limit, dateID, compareDateID);
                        } else {
                            throw new HttpException(404, "Not found");
                        }
                    }
                    string timeMarkup = d.ToString("s");
                    string timeString = d.ToString("yyyy年M月d日 H時");
                    _state = new HtmlString(string.Format("<time datetime=\"{0}\">{1}</time>", timeMarkup, timeString));
                    _isDefaultListing = (limit == 0);
                    if (!string.IsNullOrEmpty(date)) {
                        _description = string.Format("{0} {1}の戦果データ。", timeString, _server.Name);
                    }
                } else {
                    _rankingDataSet = new List<SenkaData>();
                    _state = new HtmlString("情報なし");
                }
            } else {
                throw new HttpException(404, "Not found");
            }
        }
    }
}