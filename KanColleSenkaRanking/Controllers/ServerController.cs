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
        [MvcSiteMapNodeAttribute(ParentKey = "Server", DynamicNodeProvider = "KanColleSenkaRanking.SiteMap.ServerInfoDynamicNodeProvider, KanColleSenkaRanking")]
        public ActionResult Info(int serverID) {
            if (serverID == 0) {
                return View("InfoAll");
            } else {
                ServerInfoViewModel model = new ServerInfoViewModel(serverID);
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
        [MvcSiteMapNodeAttribute(ParentKey = "Server", DynamicNodeProvider = "KanColleSenkaRanking.SiteMap.ServerRankingDynamicNodeProvider, KanColleSenkaRanking")]
        public ActionResult Ranking(int serverID, int lm = 0, string d = null) {
            //lm = limit, d = date
            if (serverID == 0) {
                return View("RankingAll");
            } else {
                ServerRankingViewModel model = new ServerRankingViewModel(serverID, lm, d);
                return View(model);
            }
        }
    }
}