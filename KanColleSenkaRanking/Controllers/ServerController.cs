using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KanColleSenkaRanking.ViewModels;
using MvcSiteMapProvider;
using System.Web.UI;
using KanColleSenkaRanking.Models;

namespace KanColleSenkaRanking.Controllers
{
    public class ServerController : Controller
    {
        private SenkaManager serverManager = DependencyResolver.Current.GetService<SenkaManager>();

#if !DEBUG
        [OutputCache(Duration = 600, VaryByParam = "serverID", Location = OutputCacheLocation.Server)]
#endif
        [MvcSiteMapNodeAttribute(ParentKey = "Server", Protocol = "https",
            DynamicNodeProvider = "KanColleSenkaRanking.SiteMap.ServerInfoDynamicNodeProvider, KanColleSenkaRanking")]
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
        [OutputCache(Duration = 1200, VaryByParam = "serverID;p;lm;d;f", Location = OutputCacheLocation.Server)]
#endif
        [MvcSiteMapNodeAttribute(ParentKey = "Server", Protocol = "https",
            DynamicNodeProvider = "KanColleSenkaRanking.SiteMap.ServerRankingDynamicNodeProvider, KanColleSenkaRanking")]
        public ActionResult Ranking(int serverID, int p = 1, int lm = 0, string d = null, string f = null) {
            //lm = limit, d = date, p = page
            if (serverID == 0) {
                if (f == "json") {
                    return HttpNotFound();
                }
                int max = 10000;
                var model = serverManager.GetAllServerRanking(max).Skip((p - 1) * max / 10).Take(max / 10).ToList();
                ViewBag.Page = p;
                return View("RankingAll", model);
            } else {
                ServerRankingViewModel model = new ServerRankingViewModel(serverID, lm, d);
                if (f == "json") {
                    return Json(model.ToJsonObject(), JsonRequestBehavior.AllowGet);
                } else {
                    return View(model);
                }
            }
        }
    }
}