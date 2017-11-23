using JrzAsp.Lib.ProtoCms.Content.Models;

namespace JrzAsp.Lib.ProtoCms.Fields.Text {
    public class TextFieldConfiguration : ContentFieldConfiguration {
        private int _maxSummaryLength;
        public string InitialValue { get; set; }
        public string DefaultValue { get; set; }
        public TextFieldEditorType EditorType { get; set; } = TextFieldEditorType.Standard;
        public int MaxSummaryLength {
            get {
                if (_maxSummaryLength < 1) _maxSummaryLength = 30;
                return _maxSummaryLength;
            }
            set => _maxSummaryLength = value;
        }
    }

    public enum TextFieldEditorType {
        Standard,
        TextArea,
        RichHtml
    }
}