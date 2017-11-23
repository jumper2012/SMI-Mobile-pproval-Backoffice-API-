using System.Web.Mvc;

namespace JrzAsp.Areas.SampleArea {
    public class SampleAreaAreaRegistration : AreaRegistration {
        public override string AreaName => "SampleArea";

        public override void RegisterArea(AreaRegistrationContext context) {
            context.MapRoute(
                "SampleArea_default",
                "SampleArea/{controller}/{action}/{id}",
                new {action = "Index", id = UrlParameter.Optional}
            );
        }
    }
}