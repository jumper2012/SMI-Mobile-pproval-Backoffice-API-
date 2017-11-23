using System.ComponentModel.DataAnnotations;

namespace JrzAsp.Lib.ProtoCms.Fields.Select.API {
    public class SelectFieldOptionGetDisplayMultiApiRequest {
        private string[] _optionValues;

        [Required]
        public string[] OptionValues {
            get {
                if (_optionValues != null) return _optionValues;
                _optionValues = new string[0];
                return _optionValues;
            }
            set => _optionValues = value;
        }

        public string HandlerParam { get; set; }
    }
}