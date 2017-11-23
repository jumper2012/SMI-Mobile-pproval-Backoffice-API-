using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using JrzAsp.Lib.EntityFrameworkUtilities;
using JrzAsp.Lib.ProtoCms;
using WebApp.Migrations;
using WebApp.Models;

namespace WebApp {
    public class MvcApplication : HttpApplication {
        private static readonly object _dbMigrationLock = new object();
        protected void Application_Start() {
            lock (_dbMigrationLock) {
                if (GlobalWebConfig.DbMigration_MigrateOnAppStart) {
                    var dbMigrator = new DatabaseMigrator<ApplicationDbContext, DbMigrationConfiguration>();
                    dbMigrator.MigrateToLatestVersion(GlobalWebConfig.DbMigration_AllowDataLoss);
                }
            }

            // Start ProtoCMS engine
            ProtoEngine.Start();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}