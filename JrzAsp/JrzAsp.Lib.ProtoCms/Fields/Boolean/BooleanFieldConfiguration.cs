using JrzAsp.Lib.ProtoCms.Content.Models;

namespace JrzAsp.Lib.ProtoCms.Fields.Boolean {
    public class BooleanFieldConfiguration : ContentFieldConfiguration {
        public bool? InitialValue { get; set; }
        public bool? DefaultValue { get; set; }
        public string NullBoolLabel { get; set; }
        public string TrueBoolLabel { get; set; }
        public string FalseBoolLabel { get; set; }
    }
}