using KanColleSenkaRanking.Models;
using MvcSiteMapProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace KanColleSenkaRanking.Controllers
{
    public class PlayerController : Controller
    {
        private SenkaManager serverManager = DependencyResolver.Current.GetService<SenkaManager>();

#if !DEBUG
        [OutputCache(Duration = 120, VaryByParam = "playerID", Location = OutputCacheLocation.Server)]
#endif
        [MvcSiteMapNodeAttribute(DynamicNodeProvider = "KanColleSenkaRanking.Models.PlayerDynamicNodeProvider, KanColleSenkaRanking")]
        public ActionResult Show(long playerID) {
            SenkaServerData server;
            var chartDataSet = serverManager.GetPlayerDataList(playerID, out server);
            if (server != null) {
                var charts = ChartData.GetPlayerCharts(chartDataSet, server.ID);
                ViewBag.PlayerDataSet = serverManager.GetPlayerActivityList(playerID, 3);
                ViewBag.PlayerData = chartDataSet.Last();
                ViewBag.RankPointChart = charts[0];
                ViewBag.RankingChart = charts[1];
                ViewBag.RankPointDeltaChart = charts[2];
                ViewBag.Server = server;
                return View();
            } else {
                ViewBag.PlayerID = playerID;
                return RedirectToAction("NoResult");
            }
        }

        [MvcSiteMapNodeAttribute(Title = "提督が見つかりません", ParentKey = "Home", Description = "該当する提督情報はありませんでした。",
            Attributes = @"{ ""visibility"": ""!XmlSiteMapResult"" }", VisibilityProvider = "MvcSiteMapProvider.FilteredSiteMapNodeVisibilityProvider, MvcSiteMapProvider")]
        public ActionResult NoResult() {
            return View();
        }
    }
}