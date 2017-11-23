using System.Web.Configuration;

namespace JrzAsp.Lib.Logging {
    internal static class MyAppSettings {

        private static int _minErrorHttpCode = -1;

        public static int MinErrorHttpCode {
            get {
                if (_minErrorHttpCode != -1) return _minErrorHttpCode;

                var code = WebConfigurationManager.AppSettings["JrzAsp.Lib.Logging:MinErrorHttpCode"];
                if (string.IsNullOrWhiteSpace(code)) code = "400";

                _minErrorHttpCode = int.Parse(code);
                return _minErrorHttpCode;
            }
        }

        public static string ConfigRelativePath {
            get {
                var crp = WebConfigurationManager.AppSettings["JrzAsp.Lib.Logging:ConfigRelativePath"];
                if (crp == null) return "~/App_Data/JrzAsp-Lib-Logging/log4net.config";
                return crp;
            }
        }
    }
}