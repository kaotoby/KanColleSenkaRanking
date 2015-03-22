using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FewMoe.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        [OutputCache(Duration = 36000)]
        public ActionResult DisplayError(string ErrorCode) {
            if (ViewEngines.Engines.FindView(ControllerContext, ErrorCode, null).View == null) {
                ErrorCode = "404";
            }
            Response.StatusCode = int.Parse(ErrorCode);
            Response.TrySkipIisCustomErrors = true;
            return View(ErrorCode);
        }
    }
}