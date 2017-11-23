using JrzAsp.Lib.ProtoCms.Fields.SimpleField;

namespace JrzAsp.Lib.ProtoCms.Fields.FilePicker {
    public class FilePickerFieldModifierForm : SimpleContentFieldModifierForm<string[]> {
        private string[] _val;
        public override string[] Val {
            get {
                if (_val != null) return _val;
                _val = new string[0];
                return _val;
            }
            set { _val = value; }
        }
    }
}