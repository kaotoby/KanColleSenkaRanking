using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KanColleSenkaRanking
{
    public class TimmingRequestAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            var stopwatch = new Stopwatch();
            HttpContext.Current.Items["Stopwatch1"] = stopwatch;
            stopwatch.Start();
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext) {
            Stopwatch stopwatch =
              (Stopwatch)HttpContext.Current.Items["Stopwatch1"];
            stopwatch.Stop();
            
            TimeSpan ts = stopwatch.Elapsed;
            string elapsedTime = ts.TotalMilliseconds.ToString();
            filterContext.Controller.ViewBag.elapsedTime = elapsedTime;

            if (HttpContext.Current.Response.ContentType == "text/html") {
                HttpContext.Current.Response.Write("<script>$(\"#elapsed\").text(" + elapsedTime + ");</script></body></html>");
            }
        }
    }
}