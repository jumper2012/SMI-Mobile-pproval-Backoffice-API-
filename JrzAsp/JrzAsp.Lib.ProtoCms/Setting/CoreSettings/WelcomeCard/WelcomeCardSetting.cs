using JrzAsp.Lib.ProtoCms.Setting.Models;

namespace JrzAsp.Lib.ProtoCms.Setting.CoreSettings.WelcomeCard {
    public class WelcomeCardSetting : SiteSetting {
        public WelcomeCardSetting(string welcomeHeading, string welcomeBody) {
            WelcomeHeading = welcomeHeading;
            WelcomeBody = welcomeBody;
        }

        public string WelcomeHeading { get; }
        public string WelcomeBody { get; }
    }
}