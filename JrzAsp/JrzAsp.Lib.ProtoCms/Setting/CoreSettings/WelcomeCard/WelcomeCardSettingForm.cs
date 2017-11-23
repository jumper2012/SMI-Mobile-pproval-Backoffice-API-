using System.ComponentModel.DataAnnotations;
using JrzAsp.Lib.ProtoCms.Setting.Models;

namespace JrzAsp.Lib.ProtoCms.Setting.CoreSettings.WelcomeCard {
    public class WelcomeCardSettingForm : SiteSettingModifierForm {
        [Required]
        [MaxLength(128)]
        public string WelcomeHeading { get; set; }

        [Required]
        [MaxLength(1024)]
        public string WelcomeBody { get; set; }
    }
}