using System.Web.Http;

namespace JrzAsp.Lib.ProtoCms.WebApi.Routing {
    public class RoutePrefixProtoCmsAttribute : RoutePrefixAttribute {

        public RoutePrefixProtoCmsAttribute(string prefix) : base(prefix) { }
        public override string Prefix => $"{ProtoCmsAppSettings.ApiRoutePrefix.Trim('/')}/{base.Prefix?.Trim('/')}";
    }
}