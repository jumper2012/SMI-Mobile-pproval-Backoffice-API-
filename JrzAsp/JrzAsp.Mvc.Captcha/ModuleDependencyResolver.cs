using JrzAsp.Mvc.Captcha.Services;

namespace JrzAsp.Mvc.Captcha {
    public static class ModuleDependencyResolver {

        public delegate ICaptchaService FunctionGetCaptchaService();

        static ModuleDependencyResolver() {
            GetCaptchaService = DefaultGetCaptchaService;
        }

        public static FunctionGetCaptchaService GetCaptchaService { get; set; }

        public static FunctionGetCaptchaService DefaultGetCaptchaService => () => new DefaultCaptchaService();
    }
}