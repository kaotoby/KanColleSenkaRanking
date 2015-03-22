using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KanColleSenkaRanking.Models;
using MvcSiteMapProvider;
using System.Web.UI;

namespace KanColleSenkaRanking.Controllers
{
    public class ServerController : Controller
    {
        private SenkaManager serverManager = DependencyResolver.Current.GetService<SenkaManager>();

#if !DEBUG
        [OutputCache(Duration = 120, VaryByParam = "serverID,lm", Location = OutputCacheLocation.Server)]
#endif
        [MvcSiteMapNodeAttribute(ParentKey = "Server", DynamicNodeProvider = "KanColleSenkaRanking.Models.ServerDynamicNodeProvider, KanColleSenkaRanking")]
        public ActionResult Show(int serverID = 0, int lm = 0) {
            //lm = limit
            if (serverID == 0) {
                return View("Dashboard");
            } else if (serverManager.Servers.Keys.Contains(serverID)) {
                SenkaServerData server = serverManager.Servers[serverID];
                IList<SenkaData> dataset;
                if (lm == 0) {
                    dataset = serverManager.GetDefaultRankingList(serverID);
                    ViewBag.DefaultList = true;
                } else {
                    dataset = serverManager.GetRankingList(serverID, lm);
                    ViewBag.DefaultList = false;
                }
                ViewBag.SenkaDataSet = dataset;
                ViewBag.Server = server;
                return View();
            } else {
                throw new HttpException(404, "Not found");
            }
        }
    }
}