using System;
using System.Web;

namespace JrzAsp.Lib.Logging.Modules {
    public class ErrorLoggerHttpModule : IHttpModule {
        public void Init(HttpApplication webApp) {
            webApp.Error += WebApp_Error;
        }

        public void Dispose() {
            // none needed disposing
        }

        private void WebApp_Error(object sender, EventArgs e) {
            var context = sender as HttpApplication;
            var logger = LogService.GetLogger(sender.GetType());
            var excp = context.Server.GetLastError();
            // will logs all error, but if it's http error, only logs it if its status code is >= MinErrorHttpCode
            if (excp == null) return;
            if (!(excp is HttpException) ||
                (excp as HttpException).GetHttpCode() >= MyAppSettings.MinErrorHttpCode) {
                logger.Error($"Web app error ({excp.GetType().FullName}): {excp.Message}", excp);
            }
        }
    }
}