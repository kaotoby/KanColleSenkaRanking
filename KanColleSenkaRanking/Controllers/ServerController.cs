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
        [MvcSiteMapNodeAttribute(ParentKey = "Server", DynamicNodeProvider = "KanColleSenkaRanking.Models.ServerDynamicNodeProvider, KanColleSenkaRanking")]
        public ActionResult Show(int serverID, int lm = 0, string d = null) {
            //lm = limit, d = date
            if (serverID == 0) {
                return View("Dashboard");
            } else {
                ServerViewModule module = new ServerViewModule(serverID, lm, d);
                ViewBag.Server = module.Server;
                return View(module);
            }
        }
    }
}