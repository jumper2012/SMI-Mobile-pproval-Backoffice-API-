using JrzAsp.Lib.ProtoCms;
using JrzAsp.Lib.RazorTools;

namespace WebApp.Features.ProtoCmsIntegration {
    public class ProtoCmsMainUrlsProvider : IProtoCmsMainUrlsProvider {
        public string GenerateManageSiteSettingUrl(string siteSettingId) {
            return UrlHelperGlobal.Self.Action("Index", "Settings", new {area = "CmsDash", id = siteSettingId});
        }

        public string GenerateManageDatumTypeUrl(string datumTypeId) {
            return UrlHelperGlobal.Self.Action("Index", "Data", new {area = "CmsDash", id = datumTypeId});
        }

        public string GenerateManageContentTypeUrl(string contentTypeId) {
            return UrlHelperGlobal.Self.Action("Index", "Contents", new {area = "CmsDash", id = contentTypeId});
        }

        public string GenerateManageFileExplorerUrl() {
            return UrlHelperGlobal.Self.Action("Index", "FileExplorer", new {area = "CmsDash", id = ""});
        }

        public string GetBaseWebsiteContentUrl() {
            return UrlHelperGlobal.Self.Content("/");
        }
    }
}