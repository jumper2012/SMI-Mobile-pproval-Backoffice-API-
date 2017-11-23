using JrzAsp.Lib.ProtoCms.Content.Models;

namespace JrzAsp.Lib.ProtoCms.Fields.SimpleField {
    public class SimpleFieldRequiredValidatorConfiguration : ContentFieldValidatorConfiguration {
        private string _requiredNullableErrorMessage;
        private string _requiredStringErrorMessage;
        public string RequiredNullableErrorMessage {
            get {
                if (_requiredNullableErrorMessage == null) {
                    _requiredNullableErrorMessage = "Field '{0}' is required.";
                }
                return _requiredNullableErrorMessage;
            }
            set => _requiredNullableErrorMessage = value;
        }
        public string RequiredStringErrorMessage {
            get {
                if (_requiredStringErrorMessage == null) {
                    _requiredStringErrorMessage = "Field '{0}' is required and not just whitespaces.";
                }
                return _requiredStringErrorMessage;
            }
            set => _requiredStringErrorMessage = value;
        }
    }
}