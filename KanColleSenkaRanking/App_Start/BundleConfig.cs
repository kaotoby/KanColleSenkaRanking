using System.Web;
using System.Web.Optimization;

namespace KanColleSenkaRanking
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles) {

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/ichart").Include(
                      "~/Scripts/ichart.{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/viewmodule").Include(
                      "~/Scripts/viewmodule/viewmodule.js"));

            bundles.Add(new ScriptBundle("~/bundles/player-chart").Include(
                      "~/Scripts/viewmodule/player/*.js"));

            bundles.Add(new StyleBundle("~/styles/bootstrap")
                .Include("~/Content/bootstrap.css"));


            bundles.Add(new StyleBundle("~/styles/css").Include(
                      "~/Content/Site.css",
                      "~/Content/Text.css"));

#if !DEBUG
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}
