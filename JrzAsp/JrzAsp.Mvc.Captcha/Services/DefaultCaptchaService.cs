using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JrzAsp.Mvc.Captcha.Models;

namespace JrzAsp.Mvc.Captcha.Services {
    public class DefaultCaptchaService : ICaptchaService {
        private const float MAX_NUM_RATIO = 0.5f;
        private const int MAX_CAPTCHA_AGE_HOUR = 1;
        private const int CAPTCHA_CHARLENGTH = 6;
        public const string DEFAULT_CAPTCHA_ID = "X______GLOBAL___CAPTCHA______X";
        private static readonly string _captchaSessionKey = $"{typeof(ICaptchaService)}:CaptchaDetails";

        private static readonly string _captchaIdViewDataDictionaryKey =
            $"{typeof(ICaptchaService)}:HtmlFieldCaptchaId";

        private readonly Random _randomizer;

        public DefaultCaptchaService() {
            _randomizer = new Random();
        }

        private static char[] ValidChars => "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"
            .ToCharArray();

        private static char[] NumericChars => "0123456789".ToCharArray();

        public CaptchaDetail GetCaptcha(string id = null, bool refresh = false) {
            id = string.IsNullOrWhiteSpace(id) ? DEFAULT_CAPTCHA_ID : id;

            var cdd = GetCaptchaDetails();
            CaptchaDetail cdt = null;
            if (cdd.TryGetValue(id, out cdt)) {
                if (DateTime.UtcNow - cdt.CreatedUtc <= TimeSpan.FromHours(MAX_CAPTCHA_AGE_HOUR) && !refresh) {
                    return cdt;
                }
            }
            cdt = new CaptchaDetail(GenerateRandomString(CAPTCHA_CHARLENGTH)) {
                UseGrayscale = MyAppSettings.UseGrayscale
            };
            cdd[id] = cdt;
            return cdt;
        }

        public string MvcAreaName => MyAppSettings.AreaName;

        public bool CheckCaptcha(string userInputCaptcha, string id = null) {
            id = string.IsNullOrWhiteSpace(id) ? DEFAULT_CAPTCHA_ID : id;

            var cdt = GetCaptcha(id);
            var checkSuccess = cdt.Text == userInputCaptcha;
            var cdd = GetCaptchaDetails();
            cdd.Remove(id);
            return checkSuccess;
        }

        public string GetCaptchaIdFromViewDataDictionary(ViewDataDictionary vdd) {
            return vdd[_captchaIdViewDataDictionaryKey] as string;
        }

        public string GetCaptchaIdFromViewDataDictionary(Dictionary<string, object> vdd) {
            return vdd[_captchaIdViewDataDictionaryKey] as string;
        }

        public void SetCaptchaIdToViewDataDictionary(ViewDataDictionary vdd, string captchaId) {
            vdd[_captchaIdViewDataDictionaryKey] = captchaId;
        }

        public void SetCaptchaIdToViewDataDictionary(Dictionary<string, object> vdd, string captchaId) {
            vdd[_captchaIdViewDataDictionaryKey] = captchaId;
        }

        private Dictionary<string, CaptchaDetail> GetCaptchaDetails() {
            var sess = HttpContext.Current.Session;
            if (sess == null) return new Dictionary<string, CaptchaDetail>();

            var cdd = sess[_captchaSessionKey] as Dictionary<string, CaptchaDetail>;
            if (cdd != null) return cdd;
            cdd = new Dictionary<string, CaptchaDetail>();
            sess[_captchaSessionKey] = cdd;
            return cdd;
        }

        private string GenerateRandomString(int charLength) {
            if (charLength < 1) charLength = 1;
            var numCharMax = Convert.ToInt32(Math.Round(MAX_NUM_RATIO * charLength));

            var result = new List<char>();
            var numCharCount = 0;
            while (result.Count < charLength) {
                char nextChar;
                do {
                    nextChar = ValidChars[_randomizer.Next() % ValidChars.Length];
                } while (NumericChars.Contains(nextChar) && numCharCount + 1 > numCharMax);

                if (NumericChars.Contains(nextChar)) numCharCount++;
                result.Add(nextChar);
            }

            return new string(result.ToArray());
        }
    }
}