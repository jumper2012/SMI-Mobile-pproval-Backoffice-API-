using JrzAsp.Lib.ProtoCms.Vue.Models;

namespace JrzAsp.Lib.ProtoCms.Content.Models {
    public class ContentPreviewPart {
        public ContentPreviewPart(string label, string field, VueComponentDefinition[] vues) {
            Label = label;
            Field = field;
            Vues = vues;
        }

        public string Label { get; }
        public string Field { get; }
        public VueComponentDefinition[] Vues { get; }
    }
}