using System.Collections.Generic;
using System.Web;
using JrzAsp.Lib.TypeUtilities;
using JrzAsp.Lib.ProtoCms.Vue.Models;

namespace WebApp.Features.MetronicTheme {
    public static class MetronicBreadcrumbs {
        private static readonly string KEY = $"{typeof(MetronicBreadcrumbs)}:{nameof(Breadcrumbs)}";
        private static LinkedList<VueLink> _breadcrumbs;
        public static LinkedList<VueLink> Breadcrumbs {
            get {
                if (HttpContext.Current == null) return new LinkedList<VueLink>();
                _breadcrumbs = HttpContext.Current.Items[KEY].DirectCastTo<LinkedList<VueLink>>();
                if (_breadcrumbs == null) {
                    _breadcrumbs = new LinkedList<VueLink>();
                    HttpContext.Current.Items[KEY] = _breadcrumbs;
                }
                return _breadcrumbs;
            }
        }
    }
}