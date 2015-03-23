using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KanColleSenkaRanking.Models;
using System.Web.Mvc;

namespace KanColleSenkaRanking.ViewModels
{
    public class ServerViewModule
    {
        public bool IsDefaultListing { get { return _isDefaultListing; } }
        public SenkaServerData Server { get { return _server; } }
        public IList<SenkaData> RankingDataSet { get { return _rankingDataSet; } }
        public HtmlString State { get { return _state; } }

        private bool _isDefaultListing;
        private SenkaServerData _server;
        private IList<SenkaData> _rankingDataSet;
        private HtmlString _state;
        private SenkaManager serverManager = DependencyResolver.Current.GetService<SenkaManager>();

        public ServerViewModule(int serverID, int limit) {
            if (serverManager.Servers.Keys.Contains(serverID)) {
                _server = serverManager.Servers[serverID];
                if (limit == 0) {
                    _rankingDataSet = serverManager.GetDefaultRankingList(serverID);
                    _isDefaultListing = true;
                } else {
                    _rankingDataSet = serverManager.GetRankingList(serverID, limit);
                    _isDefaultListing = false;
                }
                if (_server.Enabled) {
                    string timeMarkup = _server.LastUpdateTime.Time.ToString("s");
                    _state = new HtmlString(string.Format("<time datetime=\"{0}\">{1}</time>", timeMarkup, _server.LastUpdateTimeString));
                } else {
                    _state = new HtmlString("情報なし");
                }
            } else {
                throw new HttpException(404, "Not found");
            }
        }
    }
}