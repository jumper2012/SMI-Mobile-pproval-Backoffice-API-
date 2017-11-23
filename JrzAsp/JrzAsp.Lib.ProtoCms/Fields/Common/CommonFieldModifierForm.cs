using JrzAsp.Lib.ProtoCms.Content.Forms;

namespace JrzAsp.Lib.ProtoCms.Fields.Common {
    public class CommonFieldModifierForm : ContentModifierForm {
        public string ContentId { get; set; }
        public string ContentTypeId { get; set; }
        public string OperationName { get; set; }
    }
}