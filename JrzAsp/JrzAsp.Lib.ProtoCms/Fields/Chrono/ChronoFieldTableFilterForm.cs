using JrzAsp.Lib.ProtoCms.Content.Forms;

namespace JrzAsp.Lib.ProtoCms.Fields.Chrono {
    public class ChronoFieldTableFilterForm : ContentTableFilterForm {
        private ChronoFieldTableFilterFormItem[] _items;
        public ChronoFieldTableFilterFormItem[] Items {
            get {
                if (_items != null) return _items;
                _items = new ChronoFieldTableFilterFormItem[0];
                return _items;
            }
            set => _items = value;
        }
    }
}