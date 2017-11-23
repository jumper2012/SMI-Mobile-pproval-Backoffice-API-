using System.Web;
using System.Web.Mvc;

namespace JrzAsp.Lib.MvcTools {
    public static class UrlHelperFactory {
        private static readonly string URL_HELPER_FACTORY_CACHE_KEY =
            $"{typeof(UrlHelperFactory)}:{nameof(URL_HELPER_FACTORY_CACHE_KEY)}";

        public static UrlHelper Get() {
            var uh = HttpContext.Current.Items[URL_HELPER_FACTORY_CACHE_KEY];
            if (uh != null) return uh as UrlHelper;
            uh = new UrlHelper(HttpContext.Current.Request.RequestContext);
            HttpContext.Current.Items[URL_HELPER_FACTORY_CACHE_KEY] = uh;
            return (UrlHelper) uh;
        }
    }
}