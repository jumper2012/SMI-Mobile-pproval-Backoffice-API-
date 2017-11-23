using System;
using JrzAsp.Lib.ProtoCms.Content.Forms;

namespace JrzAsp.Lib.ProtoCms.Fields.Publishing {
    public class PublishingFieldModifierForm : ContentModifierForm {
        public DateTime? PublishedAt { get; set; }
        public DateTime? UnpublishedAt { get; set; }
    }
}