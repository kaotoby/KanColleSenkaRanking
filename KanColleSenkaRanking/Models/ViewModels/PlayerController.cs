using KanColleSenkaRanking.ViewModels;
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
#if !DEBUG
        [OutputCache(Duration = 600, VaryByParam = "playerID;f", Location = OutputCacheLocation.Server)]
#endif
        [MvcSiteMapNodeAttribute(DynamicNodeProvider = "KanColleSenkaRanking.SiteMap.PlayerDynamicNodeProvider, KanColleSenkaRanking", Protocol = "https")]
        public ActionResult Show(long playerID, string f = null) {
            PlayerViewModel model = new PlayerViewModel(playerID);
            if (model.Server == null) {
                ViewBag.PlayerID = playerID;
                return RedirectToAction("NoResult");
            } else if (f == "json") {
                return Json(model.ToJsonObject(), JsonRequestBehavior.AllowGet);
            } else {
                ViewBag.Server = model.Server;
                return View(model);
            }
        }

        [MvcSiteMapNodeAttribute(Title = "提督が見つかりません", ParentKey = "Home", Description = "該当する提督情報はありませんでした。", Protocol = "https",
            Attributes = @"{ ""visibility"": ""!XmlSiteMapResult"" }", VisibilityProvider = "MvcSiteMapProvider.FilteredSiteMapNodeVisibilityProvider, MvcSiteMapProvider")]
        public ActionResult NoResult() {
            return View();
        }
    }
}