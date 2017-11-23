using System.ComponentModel.DataAnnotations;
using JrzAsp.Lib.ProtoCms.Content.Forms;

namespace WebApp.Features.ApplicationDatumTypes.Role {
    public class RoleDatumModifierForm : ContentModifierForm {
        [MaxLength(256)]
        public string Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        [MaxLength(512)]
        public string Description { get; set; }

        private string[] _permissions;
        public string[] Permissions {
            get {
                if (_permissions != null) return _permissions;
                _permissions = new string[0];
                return _permissions;
            }
            set { _permissions = value; }
        }
    }
}