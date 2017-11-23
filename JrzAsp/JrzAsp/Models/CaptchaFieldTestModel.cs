using System.ComponentModel.DataAnnotations;
using JrzAsp.Mvc.Captcha.Validators;

namespace JrzAsp.Models {
    public class CaptchaFieldTestModel {
        public const string CAPTCHA_ID = "CaptchaFieldTest";

        [Required]
        [CaptchaMustMatch(CAPTCHA_ID)]
        public string CaptchaUserInput { get; set; }
    }
}