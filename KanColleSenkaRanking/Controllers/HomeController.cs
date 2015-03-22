using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KanColleSenkaRanking.Models;
using MvcSiteMapProvider;

namespace KanColleSenkaRanking.Controllers
{
    public class HomeController : Controller
    {
        private SenkaManager serverManager = DependencyResolver.Current.GetService<SenkaManager>();

#if !DEBUG
        [OutputCache(Duration = 3600)]
#endif
        public ActionResult Index() {
            ViewBag.DevStateData = DevStateData.GetFromFile();
            ViewBag.ServerList = serverManager.Servers.Values;
            return View();
        }


        [MvcSiteMapNodeAttribute(Title = "開発ツール", ParentKey = "Home",
            Description = "戦果基地使用された開発ツール。")]
        public ActionResult DevTool() {
            return View();
        }
    }
}