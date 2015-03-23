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
        [OutputCache(Duration = 600, VaryByParam = "playerID", Location = OutputCacheLocation.Server)]
#endif
        [MvcSiteMapNodeAttribute(DynamicNodeProvider = "KanColleSenkaRanking.Models.PlayerDynamicNodeProvider, KanColleSenkaRanking")]
        public ActionResult Show(long playerID) {
            PlayerViewModule module = new PlayerViewModule(playerID);
            if (module.Server != null) {
                return View(module);
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