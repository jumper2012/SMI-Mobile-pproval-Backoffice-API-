using JrzAsp.Lib.ProtoCms.Content.Forms;

namespace JrzAsp.Lib.ProtoCms.Fields.Number {
    public class NumberFieldTableFilterForm : ContentTableFilterForm {
        private NumberFieldTableFilterFormItem[] _items;
        public NumberFieldTableFilterFormItem[] Items {
            get {
                if (_items != null) return _items;
                _items = new NumberFieldTableFilterFormItem[0];
                return _items;
            }
            set => _items = value;
        }
    }
}