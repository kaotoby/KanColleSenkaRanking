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

        public ActionResult Index() {
            return View();
        }

        [ChildActionOnly] //no cache
        public PartialViewResult Announcement() {
            return PartialView();
        }

        [ChildActionOnly]
        [OutputCache(Duration = 7200)] //2 hour
        public PartialViewResult DevState() {
            ViewBag.DevStateData = DevStateData.GetFromFile();
            return PartialView();
        }

        [ChildActionOnly]
        [OutputCache(Duration = 7200)] //2 hour
        public PartialViewResult ServerList() {
            ViewBag.ServerList = serverManager.Servers.Values;
            return PartialView();
        }

        //rarely access, no cache
        [MvcSiteMapNodeAttribute(Title = "開発ツール", ParentKey = "Home",
            Description = "戦果基地使用された開発ツール。")]
        public ActionResult DevTool() {
            return View();
        }
    }
}