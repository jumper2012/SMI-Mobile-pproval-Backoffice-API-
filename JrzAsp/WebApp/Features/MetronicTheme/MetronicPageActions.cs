using System.Collections.Generic;
using System.Web;
using JrzAsp.Lib.TypeUtilities;
using JrzAsp.Lib.ProtoCms.Vue.Models;

namespace WebApp.Features.MetronicTheme {
    public static class MetronicPageActions {
        private static readonly string KEY = $"{typeof(MetronicPageActions)}:{nameof(PageActions)}";
        private static LinkedList<VueActionTrigger> _pageActions;
        public static LinkedList<VueActionTrigger> PageActions {
            get {
                if (HttpContext.Current == null) return new LinkedList<VueActionTrigger>();
                _pageActions = HttpContext.Current.Items[KEY].DirectCastTo<LinkedList<VueActionTrigger>>();
                if (_pageActions == null) {
                    _pageActions = new LinkedList<VueActionTrigger>();
                    HttpContext.Current.Items[KEY] = _pageActions;
                }
                return _pageActions;
            }
        }
    }
}