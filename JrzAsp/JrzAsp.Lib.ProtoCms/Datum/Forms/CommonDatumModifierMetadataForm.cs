using System.ComponentModel.DataAnnotations;
using JrzAsp.Lib.ProtoCms.Content.Forms;

namespace JrzAsp.Lib.ProtoCms.Datum.Forms {
    public class CommonDatumModifierMetadataForm : ContentModifierForm {
        [Required]
        [MaxLength(128)]
        public string DatumId { get; set; }
        public string DatumTypeId { get; set; }
        public string OperationName { get; set; }
    }
}