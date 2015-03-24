using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace KanColleSenkaRanking
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.LowercaseUrls = true;
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Error",
                url: "Error/{errorCode}",
                defaults: new { controller = "Error", action = "DisplayError", errorCode = "404" }
            );

            routes.MapRoute(
                name: "Server",
                url: "Server/{serverID}",
                defaults: new { controller = "Server", action = "Show", serverID = 0 }
            );

            routes.MapRoute(
                name: "NoPlayerResult",
                url: "Player/NoResult",
                defaults: new { controller = "Player", action = "NoResult" }
            );

            routes.MapRoute(
                name: "Player",
                url: "Player/{playerID}",
                defaults: new { controller = "Player", action = "Show", playerID = 0 }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
