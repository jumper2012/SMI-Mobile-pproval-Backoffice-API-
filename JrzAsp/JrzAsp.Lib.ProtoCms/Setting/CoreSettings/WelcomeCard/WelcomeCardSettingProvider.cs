using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Setting.Models;
using JrzAsp.Lib.ProtoCms.Setting.Services;

namespace JrzAsp.Lib.ProtoCms.Setting.CoreSettings.WelcomeCard {
    public class WelcomeCardSettingProvider : ISiteSettingsProvider {
        public const string WELCOME_CARD_SETTING_ID = "welcome-card";

        public IEnumerable<SiteSettingSpec> DefineSiteSettingSpecs() {
            yield return new SiteSettingSpec(
                WELCOME_CARD_SETTING_ID,
                "Welcome Card",
                "Adjust default welcome card heading and text to better greet the CMS users.",
                typeof(WelcomeCardSetting),
                typeof(WelcomeCardSettingHandler)
            );
        }
    }
}