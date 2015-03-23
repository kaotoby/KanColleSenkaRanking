using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KanColleSenkaRanking.Models;
using KanColleSenkaRanking.ViewModels;
using MvcSiteMapProvider;
using System.Web.UI;

namespace KanColleSenkaRanking.Controllers
{
    public class ServerController : Controller
    {
#if !DEBUG
        [OutputCache(Duration = 600, VaryByParam = "serverID,lm", Location = OutputCacheLocation.Server)]
#endif
        [MvcSiteMapNodeAttribute(ParentKey = "Server", DynamicNodeProvider = "KanColleSenkaRanking.Models.ServerDynamicNodeProvider, KanColleSenkaRanking")]
        public ActionResult Show(int serverID = 0, int lm = 0) {
            //lm = limit
            if (serverID == 0) {
                return View("Dashboard");
            } else {
                ServerViewModule module = new ServerViewModule(serverID, lm);
                return View(module);
            }
        }
    }
}