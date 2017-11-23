using System.ComponentModel.DataAnnotations;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.TypeUtilities;

namespace WebApp.Features.ApplicationDatumTypes.User {
    public class UserDatumModifierForm : ContentModifierForm {

        private string[] _photoUrl;

        private string[] _roleIds;

        [MaxLength(128)]
        public string Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string UserName { get; set; }

        [Required]
        public bool IsActivated { get; set; }

        [Required]
        [MaxLength(256)]
        public string DisplayName { get; set; }

        [CustomValidation(typeof(UserDatumModifierForm), "ValidatePhotoUrl")]
        public string[] PhotoUrl {
            get {
                if (_photoUrl != null) return _photoUrl;
                _photoUrl = new string[0];
                return _photoUrl;
            }
            set => _photoUrl = value;
        }

        [Required]
        [MaxLength(256)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public bool EmailConfirmed { get; set; }

        public string PhoneNumber { get; set; }

        [Required]
        public bool PhoneNumberConfirmed { get; set; }

        public string[] RoleIds {
            get {
                if (_roleIds != null) return _roleIds;
                _roleIds = new string[0];
                return _roleIds;
            }
            set => _roleIds = value;
        }

        [MaxLength(256)]
        public string Password { get; set; }

        [MaxLength(256)]
        public string PasswordConfirmation { get; set; }

        [Required]
        public bool ChangePassword { get; set; }

        public static ValidationResult ValidatePhotoUrl(object value, ValidationContext context) {
            var pus = value.DirectCastTo<string[]>();
            var photoUrl = pus.Length > 0 ? pus[0] : null;
            if (string.IsNullOrWhiteSpace(photoUrl)) {
                context.ObjectType.GetProperty(nameof(photoUrl))?.SetValue(context.ObjectInstance, new string[0]);
                return ValidationResult.Success;
            }
            var lpu = photoUrl.ToLowerInvariant();
            var validExt = new[] {".jpg", ".jpeg", ".png"};
            var hasValidExt = false;
            foreach (var ve in validExt) {
                if (lpu.EndsWith(ve)) {
                    hasValidExt = true;
                    break;
                }
            }
            if (!hasValidExt) {
                return new ValidationResult($"Photo must be {string.Join(", ", validExt)} file.",
                    new[] {nameof(PhotoUrl)});
            }
            return ValidationResult.Success;
        }
    }
}