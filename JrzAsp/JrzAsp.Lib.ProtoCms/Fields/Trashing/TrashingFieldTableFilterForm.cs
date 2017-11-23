using JrzAsp.Lib.ProtoCms.Content.Forms;

namespace JrzAsp.Lib.ProtoCms.Fields.Trashing {
    public class TrashingFieldTableFilterForm : ContentTableFilterForm {
        public bool IsTrashed { get; set; }
    }
}