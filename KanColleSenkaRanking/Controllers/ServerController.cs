using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KanColleSenkaRanking.ViewModels;
using MvcSiteMapProvider;
using System.Web.UI;

namespace KanColleSenkaRanking.Controllers
{
    public class ServerController : Controller
    {

#if !DEBUG
        [OutputCache(Duration = 600, VaryByParam = "serverID;lm;d", Location = OutputCacheLocation.Server)]
#endif
        [MvcSiteMapNodeAttribute(ParentKey = "Server", DynamicNodeProvider = "KanColleSenkaRanking.Models.ServerInfoDynamicNodeProvider, KanColleSenkaRanking")]
        public ActionResult Info(int serverID) {
            ServerInfoViewModel model = new ServerInfoViewModel(serverID);
            ViewBag.Server = model.Server;
            if (serverID == 0) {
                return View("InfoAll");
            } else {
                if (model.Server.Enabled) {
                    return View(model);
                } else {
                    return View("InfoNoData", model);
                }
            }
        }

#if !DEBUG
        [OutputCache(Duration = 600, VaryByParam = "serverID;lm;d", Location = OutputCacheLocation.Server)]
#endif
        [MvcSiteMapNodeAttribute(ParentKey = "Server", DynamicNodeProvider = "KanColleSenkaRanking.Models.ServerRankingDynamicNodeProvider, KanColleSenkaRanking")]
        public ActionResult Ranking(int serverID, int lm = 0, string d = null) {
            //lm = limit, d = date
            if (serverID == 0) {
                return View("RankingAll");
            } else {
                ServerRankingViewModel model = new ServerRankingViewModel(serverID, lm, d);
                ViewBag.Server = model.Server;
                return View(model);
            }
        }
    }
}