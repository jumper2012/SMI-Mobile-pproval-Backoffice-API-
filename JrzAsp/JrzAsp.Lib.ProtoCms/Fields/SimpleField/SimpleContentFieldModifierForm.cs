using JrzAsp.Lib.ProtoCms.Content.Forms;

namespace JrzAsp.Lib.ProtoCms.Fields.SimpleField {
    public abstract class SimpleContentFieldModifierForm<TVal> : ContentModifierForm {
        public virtual TVal Val { get; set; }
    }
}