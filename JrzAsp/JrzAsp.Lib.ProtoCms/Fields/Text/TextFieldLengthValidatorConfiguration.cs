using JrzAsp.Lib.ProtoCms.Content.Models;

namespace JrzAsp.Lib.ProtoCms.Fields.Text {
    public class TextFieldLengthValidatorConfiguration : ContentFieldValidatorConfiguration {
        private string _maxLengthErrorMessage;
        private string _minLengthErrorMessage;
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public string MinLengthErrorMessage {
            get {
                if (_minLengthErrorMessage == null) {
                    _minLengthErrorMessage = "Field '{0}' minimum length should be {1} char(s).";
                }
                return _minLengthErrorMessage;
            }
            set => _minLengthErrorMessage = value;
        }
        public string MaxLengthErrorMessage {
            get {
                if (_maxLengthErrorMessage == null) {
                    _maxLengthErrorMessage = "Field '{0}' maximum length should be {1} char(s).";
                }
                return _maxLengthErrorMessage;
            }
            set => _maxLengthErrorMessage = value;
        }
    }
}