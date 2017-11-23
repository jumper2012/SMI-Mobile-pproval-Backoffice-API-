using System.Linq;
using System.Web.Configuration;

namespace JrzAsp.Lib.ErrorRazor {
    internal static class MyAppSettings {
        private static string[] _handledStatusCodes;

        private static string _errorViewNamePrefix;

        public static string[] HandledStatusCodes {
            get {
                if (_handledStatusCodes != null) return _handledStatusCodes;

                var handled = WebConfigurationManager.AppSettings["JrzAsp.Lib.ErrorRazor:HandledStatusCodes"];
                if (string.IsNullOrWhiteSpace(handled)) handled = "4xx,5xx";

                _handledStatusCodes = handled.Split(',', ';')
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .ToArray();

                return _handledStatusCodes;
            }
        }

        public static string ErrorViewNamePrefix {
            get {
                if (_errorViewNamePrefix != null) return _errorViewNamePrefix;

                var prefix = WebConfigurationManager.AppSettings["JrzAsp.Lib.ErrorRazor:ErrorViewNamePrefix"];
                if (string.IsNullOrWhiteSpace(prefix)) prefix = "_error_view_";
                _errorViewNamePrefix = prefix;
                return _errorViewNamePrefix;
            }
        }
        
        public static bool Enable {
            get {
                var boolStr = WebConfigurationManager.AppSettings["JrzAsp.Lib.ErrorRazor:Enable"];
                if (bool.TryParse(boolStr, out var yes)) {
                    return yes;
                }
                return false;
            }
        }
    }
}