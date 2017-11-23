using System.Web.Configuration;

namespace JrzAsp.Mvc.Captcha {
    internal static class MyAppSettings {
        private static string _areaName;

        private static string _baseRoute;

        private static string _areasFolderRelativePath;

        private static string _captchaFieldViewName;

        public static string AreaName {
            get {
                if (_areaName != null) return _areaName;
                var name = WebConfigurationManager.AppSettings["JrzAsp.Mvc.Captcha:AreaName"];
                if (string.IsNullOrWhiteSpace(name)) name = "JrzAsp-Mvc-Captcha";
                _areaName = name;
                return _areaName;
            }
        }

        public static string BaseRoute {
            get {
                if (_baseRoute != null) return _baseRoute;
                var route = WebConfigurationManager.AppSettings["JrzAsp.Mvc.Captcha:BaseRoute"];
                if (string.IsNullOrWhiteSpace(route)) route = AreaName;
                _baseRoute = route;
                return _baseRoute;
            }
        }

        public static string AreasFolderRelativePath {
            get {
                if (_areasFolderRelativePath != null) return _areasFolderRelativePath;
                var folder = WebConfigurationManager.AppSettings["JrzAsp.Mvc.Captcha:AreasFolderRelativePath"];
                if (string.IsNullOrWhiteSpace(folder)) folder = "~/Areas";
                _areasFolderRelativePath = folder;
                return _areasFolderRelativePath;
            }
        }

        public static string CaptchaFieldViewName {
            get {
                if (_captchaFieldViewName != null) return _captchaFieldViewName;
                var name = WebConfigurationManager.AppSettings["JrzAsp.Mvc.Captcha:CaptchaFieldViewName"];
                if (string.IsNullOrWhiteSpace(name)) {
                    name = $"{AreasFolderRelativePath}/{AreaName}/Views/Shared/_edit-field-captcha.cshtml";
                }
                _captchaFieldViewName = name;
                return _captchaFieldViewName;
            }
        }

        public static bool UseGrayscale {
            get {
                var name = WebConfigurationManager.AppSettings["JrzAsp.Mvc.Captcha:UseGrayscale"];
                if (string.IsNullOrWhiteSpace(name)) {
                    return false;
                }
                var use = false;
                bool.TryParse(name, out use);
                return use;
            }
        }
    }
}