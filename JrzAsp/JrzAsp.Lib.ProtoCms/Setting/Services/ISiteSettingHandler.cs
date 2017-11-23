using JrzAsp.Lib.ProtoCms.Setting.Database;
using JrzAsp.Lib.ProtoCms.Setting.Models;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.RepoPattern;

namespace JrzAsp.Lib.ProtoCms.Setting.Services {
    public interface ISiteSettingHandler : IPerRequestDependency {
        SiteSetting GetSettingObject(ProtoSettingField[] settingFields);
        SiteSettingModifierForm BuildSettingModifierForm(SiteSetting oldSetting);
        VueComponentDefinition[] CreateSettingModifierFormVues(SiteSettingModifierForm form, SiteSetting oldSetting);
        FurtherValidationResult ValidateSettingModifierForm(SiteSettingModifierForm form, SiteSetting oldSetting);
        ProtoSettingField[] UpdateAndRebuildNewSettingFields(SiteSettingModifierForm form, SiteSetting oldSetting);
    }
}