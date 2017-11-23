using System.Web.Optimization;

namespace WebApp {
    public class BundleConfig {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles) {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js",
                "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/site.css"));

            // Bootstrap 3 responsive text alignment
            bundles.Add(new StyleBundle("~/Content/bootstrap/responsive-text-alignment").Include(
                "~/Content/bootstrap-responsive-text-alignment.css"));

            // Metronic theme custom assets
            bundles.Add(new StyleBundle("~/Content/metronic-theme/custom-css").Include(
                "~/Content/MetronicTheme/app-assets/css/proto-cms.css"));

            // Lodash
            bundles.Add(new ScriptBundle("~/bundles/lodash").Include(
                "~/Scripts/lodash.js"));

            // VueJS
            bundles.Add(new ScriptBundle("~/bundles/vue").Include(
                "~/Scripts/vue.js"));
            
            // ProtoCMS
            bundles.Add(new ScriptBundle("~/bundles/protocms").Include(
                "~/Scripts/proto-cms.js"));
        }
    }
}