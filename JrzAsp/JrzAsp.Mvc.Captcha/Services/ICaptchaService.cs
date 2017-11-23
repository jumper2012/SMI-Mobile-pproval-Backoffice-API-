using System.Collections.Generic;
using System.Web.Mvc;
using JrzAsp.Mvc.Captcha.Models;

namespace JrzAsp.Mvc.Captcha.Services {
    public interface ICaptchaService {
        string MvcAreaName { get; }
        bool CheckCaptcha(string userInputCaptcha, string id = null);
        CaptchaDetail GetCaptcha(string id = null, bool refresh = false);
        string GetCaptchaIdFromViewDataDictionary(Dictionary<string, object> vdd);
        string GetCaptchaIdFromViewDataDictionary(ViewDataDictionary vdd);
        void SetCaptchaIdToViewDataDictionary(Dictionary<string, object> vdd, string captchaId);
        void SetCaptchaIdToViewDataDictionary(ViewDataDictionary vdd, string captchaId);
    }
}