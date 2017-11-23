using System.Web.Mvc;

namespace WebApp.Areas.CmsDash {
    public class CmsDashAreaRegistration : AreaRegistration {
        public override string AreaName => "CmsDash";

        public override void RegisterArea(AreaRegistrationContext context) {
            context.MapRoute(
                "CmsDash_default",
                "CmsDash/{controller}/{action}/{id}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional}
            );
        }
    }
}