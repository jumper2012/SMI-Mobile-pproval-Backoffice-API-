using JrzAsp.Lib.ProtoCms.Vue.Models;

namespace JrzAsp.Lib.ProtoCms.Fields.Select {
    public class SelectFieldOption {
        public SelectFieldOption(string value, string label, VueComponentDefinition[] vues) {
            Value = value;
            Label = label;
            Vues = vues ?? new VueComponentDefinition[0];
        }

        public string Value { get; }
        public string Label { get; }
        public VueComponentDefinition[] Vues { get; }
    }
}