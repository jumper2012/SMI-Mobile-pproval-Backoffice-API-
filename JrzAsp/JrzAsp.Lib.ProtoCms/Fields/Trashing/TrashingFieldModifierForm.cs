using System;
using JrzAsp.Lib.ProtoCms.Content.Forms;

namespace JrzAsp.Lib.ProtoCms.Fields.Trashing {
    public class TrashingFieldModifierForm : ContentModifierForm {
        public DateTime? TrashedAt { get; set; }
    }
}