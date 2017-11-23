using System.Web.Mvc;
using System.Web.Routing;

namespace WebApp {
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional},
                null,
                new[] {"WebApp.Controllers"}
            );

            // ProtoCMS' Faux URL Slug
            routes.MapRoute(
                "ProtoCMS_FauxUrlSlug",
                "{*fauxUrlSlug}",
                new {controller = "FauxUrlSlug", action = "DisplayContent"},
                null,
                new[] {"WebApp.Controllers"}
            );
        }
    }
}