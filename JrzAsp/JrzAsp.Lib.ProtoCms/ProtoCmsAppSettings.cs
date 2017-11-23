using System.Web.Configuration;

namespace JrzAsp.Lib.ProtoCms {
    public static class ProtoCmsAppSettings {
        private static string _apiRoutePrefix;
        public static string ApiRoutePrefix {
            get {
                if (_apiRoutePrefix != null) return _apiRoutePrefix;
                _apiRoutePrefix = WebConfigurationManager.AppSettings["ProtoCms:ApiRoutePrefix"]
                    ?.Trim().Trim('\\', '/');
                if (string.IsNullOrEmpty(_apiRoutePrefix)) {
                    _apiRoutePrefix = "/proto-cms-api/v1";
                } else {
                    _apiRoutePrefix = "/" + _apiRoutePrefix;
                }
                return _apiRoutePrefix;
            }
        }

        public static string WebPageBaseTitle {
            get {
                var title = WebConfigurationManager.AppSettings["ProtoCms:WebPageBaseTitle"];
                if (!string.IsNullOrWhiteSpace(title)) {
                    return title;
                }
                return "CMS Dash";
            }
        }

        public static string FooterCopyright {
            get {
                var copyright = WebConfigurationManager.AppSettings["ProtoCms:FooterCopyright"];
                if (!string.IsNullOrWhiteSpace(copyright)) {
                    return copyright;
                }
                return "Suitmedia";
            }
        }

        public static string LogoUrl {
            get {
                var logoUrl = WebConfigurationManager.AppSettings["ProtoCms:LogoUrl"];
                if (!string.IsNullOrWhiteSpace(logoUrl)) {
                    return logoUrl;
                }
                return null;
            }
        }

        public static string FileExplorerBasePath {
            get {
                var basePath = WebConfigurationManager.AppSettings["ProtoCms:FileExplorerBasePath"];
                if (!string.IsNullOrWhiteSpace(basePath)) {
                    return basePath;
                }
                return "~/Uploads";
            }
        }
    }
}