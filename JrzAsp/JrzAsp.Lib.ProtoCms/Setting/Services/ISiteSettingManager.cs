using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Setting.Models;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.RepoPattern;

namespace JrzAsp.Lib.ProtoCms.Setting.Services {
    public interface ISiteSettingManager : IGlobalSingletonDependency {
        SiteSettingSpec[] SettingSpecs { get; }
        IReadOnlyDictionary<string, ISiteSettingHandler> SettingHandlers { get; }
        
        SiteSetting GetSetting(string settingId, bool forceRefresh = false);
        TStg GetSetting<TStg>(bool forceRefresh = false) where TStg : SiteSetting;
        IReadOnlyDictionary<string, SiteSetting> GetSettings(bool forceRefresh = false);
        SiteSettingModifierForm BuildSettingModifierForm(string settingId);
        FurtherValidationResult ValidateSettingModifierForm(string settingId, SiteSettingModifierForm form);
        VueComponentDefinition[] CreateSettingModifierFormVues(string settingId, SiteSettingModifierForm form);
        void UpdateSetting(string settingId, SiteSettingModifierForm form);
    }
}