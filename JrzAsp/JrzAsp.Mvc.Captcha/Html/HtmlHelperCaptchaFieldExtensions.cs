using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using JrzAsp.Lib.RazorTools;
using JrzAsp.Mvc.Captcha.Services;

namespace JrzAsp.Mvc.Captcha.Html {
    public static class HtmlHelperCaptchaFieldExtensions {
        public static MvcHtmlString CaptchaFieldFor<TModel>(this HtmlHelper<TModel> html,
            string captchaId, Expression<Func<TModel, string>> modelCaptchaExpression,
            string viewNamePrefix = null,
            string customHtmlFieldPrefix = null,
            bool clearHtmlFieldPrefixBeforeUsingCustom = false,
            IDictionary<string, object> extraViewData = null) {
            var evd = extraViewData ?? new Dictionary<string, object>();
            evd.Add($"{typeof(ICaptchaService)}:HtmlFieldCaptchaId", captchaId);
            return html.PartialFor($"{viewNamePrefix}{MyAppSettings.CaptchaFieldViewName}", modelCaptchaExpression,
                customHtmlFieldPrefix,
                clearHtmlFieldPrefixBeforeUsingCustom, evd);
        }
    }
}