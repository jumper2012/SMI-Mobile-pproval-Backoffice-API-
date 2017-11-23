using JrzAsp.Lib.ProtoCms.Permission.Models;
using JrzAsp.Lib.ProtoCms.Setting.Models;

namespace JrzAsp.Lib.ProtoCms.Setting.Permissions {
    public class ViewSiteSettingPermission : ProtoPermission {
        public ViewSiteSettingPermission(SiteSettingSpec settingSpec) {
            SettingSpec = settingSpec;
        }

        public SiteSettingSpec SettingSpec { get; }

        public override string Id => GetIdFor(SettingSpec.Id);
        public override string DisplayName => $"View {SettingSpec.Name} Setting";

        public override string Description =>
            $"View '{SettingSpec.Name}' setting, description: {SettingSpec.Description}";

        public override string CategoryName => "Site Settings";
        public override string SubCategoryName => SettingSpec.Name;
        public override bool AuthenticatedUserHasThisByDefault => true;
        public override bool GuestUserHasThisByDefault => true;

        public static string GetIdFor(string settingId) {
            return $"SiteSetting({settingId}):View";
        }
    }
}