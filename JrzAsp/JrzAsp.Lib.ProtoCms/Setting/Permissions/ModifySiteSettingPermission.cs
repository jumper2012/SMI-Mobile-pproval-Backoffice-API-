using JrzAsp.Lib.ProtoCms.Permission.Models;
using JrzAsp.Lib.ProtoCms.Setting.Models;

namespace JrzAsp.Lib.ProtoCms.Setting.Permissions {
    public class ModifySiteSettingPermission : ProtoPermission {
        public ModifySiteSettingPermission(SiteSettingSpec settingSpec) {
            SettingSpec = settingSpec;
        }

        public SiteSettingSpec SettingSpec { get; }

        public override string Id => GetIdFor(SettingSpec.Id);
        public override string DisplayName => $"Modify {SettingSpec.Name} Setting";
        public override string Description => $"Modify '{SettingSpec.Name}' setting, description: {SettingSpec.Description}";
        public override string CategoryName => "Site Settings";
        public override string SubCategoryName => SettingSpec.Name;
        public override bool AuthenticatedUserHasThisByDefault => false;
        public override bool GuestUserHasThisByDefault => false;

        public static string GetIdFor(string settingId) {
            return $"SiteSetting({settingId}):Modify";
        }
    }
}