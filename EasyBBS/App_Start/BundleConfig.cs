using System.Web;
using System.Web.Optimization;

namespace EasyBBS 
{
    public class BundleConfig
    {
        // バンドルの詳細については、https://go.microsoft.com/fwlink/?LinkId=301862 を参照してください
        public static void RegisterBundles(BundleCollection bundles)
        {
            // jQuery のバンドル (Install-Package jQuery で導入)
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // jQuery Validation のバンドル (Install-Package jQuery.Validation で導入)
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/jquery.validate.unobtrusive.js")); // Remote Validation に必要

            // Modernizr のバンドル (Optional)
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));



            // Bootstrap CSS と Site.css のバンドル
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));


            BundleTable.EnableOptimizations = false; 
        }
    }
}