# How To Install into ASP MVC Web App #

1.  Include lib as reference (except 3rd party that you already have)
2.  ~/Areas must be writable by the webapp IIS AppPool
3.  Adjust in the webapp web.config configuration appSettings "JrzAsp.Mvc.Captcha:CaptchaFieldViewName" if you wan't to use another view for rendering the captcha.
    Please refer to ~/Areas/Jrz-Asp-Captcha/Views/Shared/_edit-field-captcha.cshtml to see default captcha view implementation.