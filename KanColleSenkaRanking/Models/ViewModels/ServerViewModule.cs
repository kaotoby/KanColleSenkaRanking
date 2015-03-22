using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KanColleSenkaRanking.Models;

namespace KanColleSenkaRanking.ViewModels
{
    public class ServerViewModule
    {
        public bool IsDefaultListing { get { return _isDefaultListing; } }
        public SenkaServerData Server { get { return _server; } }
        public IList<SenkaData> RankingDataSet { get { return _rankingDataSet; } }

        private bool _isDefaultListing;
        private SenkaServerData _server;
        private IList<SenkaData> _rankingDataSet;

        public ServerViewModule(int serverID) {
            //Not Done
        }
    }
}