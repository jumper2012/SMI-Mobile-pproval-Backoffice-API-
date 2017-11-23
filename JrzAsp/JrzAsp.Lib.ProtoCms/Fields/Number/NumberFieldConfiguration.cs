using JrzAsp.Lib.ProtoCms.Content.Models;

namespace JrzAsp.Lib.ProtoCms.Fields.Number {
    public class NumberFieldConfiguration : ContentFieldConfiguration {
        public NumberValueKind NumberKind { get; set; } = NumberValueKind.Integer;
        public decimal? InitialValue { get; set; }
        public decimal? DefaultValue { get; set; }
        private decimal _searchDecimalSensitivity = 0.01m;
        public decimal SearchDecimalSensitivity {
            get {
                if (_searchDecimalSensitivity < 0) {
                    _searchDecimalSensitivity = _searchDecimalSensitivity * -1.0m;
                }
                return _searchDecimalSensitivity;
            }
            set { _searchDecimalSensitivity = value; }
        }
    }

    public enum NumberValueKind {
        Integer,
        Decimal
    }
}