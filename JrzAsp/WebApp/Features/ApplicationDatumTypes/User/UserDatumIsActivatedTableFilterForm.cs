using JrzAsp.Lib.ProtoCms.Content.Forms;

namespace WebApp.Features.ApplicationDatumTypes.User {
    public class UserDatumIsActivatedTableFilterForm : ContentTableFilterForm {
        public bool IsActivated { get; set; }
    }
}