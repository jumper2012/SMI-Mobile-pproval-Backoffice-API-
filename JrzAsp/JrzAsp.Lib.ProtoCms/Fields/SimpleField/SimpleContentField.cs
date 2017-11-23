using JrzAsp.Lib.ProtoCms.Content.Models;

namespace JrzAsp.Lib.ProtoCms.Fields.SimpleField {
    public abstract class SimpleContentField<TVal> : ContentField {
        public virtual TVal Val { get; set; }
    }
}