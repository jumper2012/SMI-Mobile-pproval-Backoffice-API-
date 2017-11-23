using System.ComponentModel.DataAnnotations;

namespace JrzAsp.Lib.ProtoCms.Fields.Select.API {
    public class SelectFieldOptionGetDisplayApiRequest {
        [Required]
        public string OptionValue { get; set; }

        public string HandlerParam { get; set; }
    }
}