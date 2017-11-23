using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace JrzAsp.Lib.StrictAreaRegistration {
    public abstract class BaseStrictAreaRegistration : AreaRegistration {
        public abstract Assembly AreaAssembly { get; }

        public virtual string BaseRoute => AreaName;

        public abstract string AreaControllersNamespace { get; }

        public sealed override void RegisterArea(AreaRegistrationContext context) {
            var areaControllerNames = GetAreaControllerNames().ToList();
            OnBeforeRegisterArea(context, AreaName, BaseRoute, AreaControllersNamespace, areaControllerNames);
            OnRegisterArea(context, AreaName, BaseRoute, AreaControllersNamespace, areaControllerNames);
            OnAfterRegisterArea(context, AreaName, BaseRoute, AreaControllersNamespace, areaControllerNames);
        }

        protected virtual void OnRegisterArea(AreaRegistrationContext context, string areaName, string baseRoute,
            string areaControllersNamespace, IEnumerable<string> areaControllerNames) {
            context.MapRoute(
                $"{areaName}_default",
                $"{baseRoute}/{{controller}}/{{action}}/{{id}}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional},
                new {controller = string.Join("|", areaControllerNames)},
                new[] {areaControllersNamespace}
            );
        }

        protected virtual void OnBeforeRegisterArea(AreaRegistrationContext context, string areaName, string baseRoute,
            string areaControllersNamespace, IEnumerable<string> areaControllerNames) { }

        protected virtual void OnAfterRegisterArea(AreaRegistrationContext context, string areaName, string baseRoute,
            string areaControllersNamespace, IEnumerable<string> areaControllerNames) { }

        protected virtual IEnumerable<string> GetAreaControllerNames() {
            var areaControllerNames = AreaAssembly.GetTypes().Where(
                    t => t.Namespace == AreaControllersNamespace && typeof(Controller).IsAssignableFrom(t)
                         && t.IsClass && !t.IsAbstract && !t.IsInterface && !t.IsGenericType
                         && t.Name.EndsWith("Controller")
                )
                .Select(c => c.Name.Substring(0, c.Name.Length - "Controller".Length));
            return areaControllerNames;
        }
    }
}