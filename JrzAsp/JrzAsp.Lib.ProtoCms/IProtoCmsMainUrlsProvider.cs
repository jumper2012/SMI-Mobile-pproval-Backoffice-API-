namespace JrzAsp.Lib.ProtoCms {
    public interface IProtoCmsMainUrlsProvider : IGlobalSingletonDependency {
        string GenerateManageSiteSettingUrl(string siteSettingId);
        string GenerateManageDatumTypeUrl(string datumTypeId);
        string GenerateManageContentTypeUrl(string contentTypeId);
        string GenerateManageFileExplorerUrl();
        string GetBaseWebsiteContentUrl();
    }
}