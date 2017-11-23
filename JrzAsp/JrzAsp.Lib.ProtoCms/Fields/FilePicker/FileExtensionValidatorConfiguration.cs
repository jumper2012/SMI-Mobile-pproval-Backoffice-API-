using JrzAsp.Lib.ProtoCms.Content.Models;

namespace JrzAsp.Lib.ProtoCms.Fields.FilePicker {
    public class FileExtensionValidatorConfiguration : ContentFieldValidatorConfiguration {
        private string[] _allowedFileExtensions;
        public string[] AllowedFileExtensions {
            get {
                if (_allowedFileExtensions != null) return _allowedFileExtensions;
                _allowedFileExtensions = new string[0];
                return _allowedFileExtensions;
            }
            set => _allowedFileExtensions = value;
        }
    }
}