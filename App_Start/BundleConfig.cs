using System.Web;
using System.Web.Optimization;

namespace BookStore
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/libs/bootstrap/dist/js/bootstrap.js",
                      "~/libs/flexselect/dist/js/jquery.flexselect.js",
                      "~/libs/nahro/dist/js/all.js",
                      "~/libs/nahro/dist/js/FormSlides.js",
                      "~/libs/fontawesome/dist/js/all.js",
                      "~/Scripts/bootbox.js",
                      "~/Scripts/toastr.js",
                      "~/Scripts/liquidmetal.js",
                      "~/Scripts/site/site.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/libs/bootstrap/dist/css/rtl/bootstrap.css",
                      "~/libs/flexselect/dist/css/flexselect.css",
                      "~/Content/site/site.css",
                      "~/libs/fontawesome/dist/css/all.css",
                      "~/libs/nahro/dist/css/all.css",
                      "~/Content/toastr.css"));
        }
    }
}
