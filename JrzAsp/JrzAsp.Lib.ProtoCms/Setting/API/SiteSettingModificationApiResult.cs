using JrzAsp.Lib.RepoPattern;
using JrzAsp.Lib.ProtoCms.Core.Models;

namespace JrzAsp.Lib.ProtoCms.Setting.API {
    public class SiteSettingModificationApiResult : IModificationApiResult {
        private FurtherValidationResult _validationResult;
        public FurtherValidationResult ValidationResult {
            get {
                if (_validationResult == null) _validationResult = new FurtherValidationResult();
                return _validationResult;
            }
            set => _validationResult = value;
        }
        public bool IsSuccess { get; set; }
        public string SettingId { get; set; }
    }
}