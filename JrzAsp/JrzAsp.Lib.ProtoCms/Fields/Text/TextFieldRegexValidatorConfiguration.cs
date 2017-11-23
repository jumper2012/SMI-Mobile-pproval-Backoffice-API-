using System.Text.RegularExpressions;
using JrzAsp.Lib.ProtoCms.Content.Models;

namespace JrzAsp.Lib.ProtoCms.Fields.Text {
    public class TextFieldRegexValidatorConfiguration : ContentFieldValidatorConfiguration {
        private string _errorMessage;
        private string _pattern;

        private Regex _regex;
        public string Pattern {
            get => _pattern;
            set {
                _pattern = value;
                _regex = null;
            }
        }
        public string ErrorMessage {
            get {
                if (_errorMessage == null) {
                    _errorMessage = "Field '{0}' must match regex '{1}'.";
                }
                return _errorMessage;
            }
            set => _errorMessage = value;
        }
        public RegexOptions RegexOptions { get; set; } = RegexOptions.Compiled | RegexOptions.CultureInvariant;
        public Regex Regex {
            get {
                if (_regex != null) return _regex;
                var pattern = Pattern;
                if (string.IsNullOrEmpty(pattern)) pattern = ".*";
                _regex = new Regex(pattern, RegexOptions);
                return _regex;
            }
        }
    }
}