using JrzAsp.Lib.ProtoCms.Content.Forms;

namespace JrzAsp.Lib.ProtoCms.Fields.Boolean {
    public class BooleanFieldTableFilterForm : ContentTableFilterForm {
        private BooleanFieldTableFilterFormItem[] _items;
        public BooleanFieldTableFilterFormItem[] Items {
            get {
                if (_items != null) return _items;
                _items = new BooleanFieldTableFilterFormItem[0];
                return _items;
            }
            set => _items = value;
        }
    }
}