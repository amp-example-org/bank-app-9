using System.Web.Optimization;

namespace AcmeBankApp.Web.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Legacy jQuery bundles
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-1.12.4.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Legacy modernizr
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            // Bootstrap 3.3.7
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            // Angular 7 bundles - legacy manual bundling approach
            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                        "~/Scripts/angular/angular.js",
                        "~/Scripts/angular/angular-route.js"));

            // Pre-built Angular 7 app (committed to source)
            bundles.Add(new ScriptBundle("~/bundles/angular-app").Include(
                        "~/Scripts/dist/runtime.js",
                        "~/Scripts/dist/polyfills.js", 
                        "~/Scripts/dist/main.js"));

            // Legacy CSS bundles
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            // Angular app styles
            bundles.Add(new StyleBundle("~/Content/angular-css").Include(
                      "~/Scripts/dist/styles.css"));

            // Legacy: Disable bundling and minification for development
            #if DEBUG
            BundleTable.EnableOptimizations = false;
            #else
            BundleTable.EnableOptimizations = true;
            #endif
        }
    }
}
