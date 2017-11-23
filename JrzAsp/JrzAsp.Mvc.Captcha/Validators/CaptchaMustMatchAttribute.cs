using System.ComponentModel.DataAnnotations;

namespace JrzAsp.Mvc.Captcha.Validators {
    public class CaptchaMustMatchAttribute : ValidationAttribute {

        public CaptchaMustMatchAttribute(string captchaId = null) {
            CaptchaId = captchaId;
        }

        public string CaptchaId { get; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            var errMsg = string.IsNullOrWhiteSpace(ErrorMessage)
                ? "Captcha doesn't match."
                : ErrorMessage;

            var captchaSvc = ModuleDependencyResolver.GetCaptchaService();
            return !captchaSvc.CheckCaptcha(value as string, CaptchaId)
                ? new ValidationResult(errMsg, new[] {validationContext.MemberName})
                : ValidationResult.Success;
        }
    }
}