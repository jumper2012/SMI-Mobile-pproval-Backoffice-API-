using JrzAsp.Lib.ProtoCms.Setting.Services;

namespace WebApp.Areas.CmsDash.Models {
    public class SettingViewModel {
        public string SettingId { get; set; }
        public ISiteSettingManager SettingManager { get; set; }
    }
}