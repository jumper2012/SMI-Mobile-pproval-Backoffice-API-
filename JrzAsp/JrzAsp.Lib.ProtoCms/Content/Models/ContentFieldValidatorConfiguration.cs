namespace JrzAsp.Lib.ProtoCms.Content.Models {
    public class ContentFieldValidatorConfiguration {
        private string[] _modifyOperationNamesThatIgnoreValidation;
        public string[] ModifyOperationNamesThatIgnoreValidation {
            get {
                if (_modifyOperationNamesThatIgnoreValidation == null) {
                    _modifyOperationNamesThatIgnoreValidation = new string[0];
                }
                return _modifyOperationNamesThatIgnoreValidation;
            }
            set => _modifyOperationNamesThatIgnoreValidation = value;
        }
    }
}