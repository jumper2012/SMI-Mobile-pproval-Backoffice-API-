using System.ComponentModel.DataAnnotations;

namespace JrzAsp.Lib.ProtoCms.Content.Forms {
    public abstract class ContentTableFilterForm {
        [Required]
        public bool __IsFilterEnabled { get; set; }
    }
}