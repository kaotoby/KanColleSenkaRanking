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
            IEnumerable<DevStateData> model = DevStateData.GetFromFile();
            return PartialView(model);
        }

        [ChildActionOnly]
        [OutputCache(Duration = 7200)] //2 hour
        public PartialViewResult ServerList() {
            IEnumerable<SenkaServerData> model = serverManager.Servers.Values;
            return PartialView(model);
        }

        //rarely access, no cache
        [MvcSiteMapNodeAttribute(Title = "開発ツール", ParentKey = "Home",
            Description = "戦果基地使用された開発ツール。")]
        public ActionResult DevTool() {
            return View();
        }
    }
}