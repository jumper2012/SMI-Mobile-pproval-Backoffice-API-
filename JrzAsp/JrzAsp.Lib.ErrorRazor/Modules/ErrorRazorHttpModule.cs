using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using JrzAsp.Lib.DirectViewRenderer;
using JrzAsp.Lib.Logging;

namespace JrzAsp.Lib.ErrorRazor.Modules {
    public class ErrorRazorHttpModule : IHttpModule {
        private readonly ILog _logger = LogService.GetLogger(typeof(ErrorRazorHttpModule));

        public void Init(HttpApplication context) {
            context.PreSendRequestHeaders += OnPreSendRequestHeaders;
        }

        public void Dispose() {
            // none need disposing
        }

        private void OnPreSendRequestHeaders(object sender, EventArgs eventArgs) {
            if (!MyAppSettings.Enable) return;
            if (HttpContext.Current == null) return;

            var req = HttpContext.Current.Request;
            var res = HttpContext.Current.Response;

            if (!res.ContentType.Contains("text/html")) return;
            if (req.AcceptTypes != null && !req.AcceptTypes.Contains("text/html")) return;
            if (!IsHandledStatusCode(res.StatusCode)) return;

            var server = HttpContext.Current.Server;
            var theError = server.GetLastError();
            var statusCode = HttpContext.Current.Response.StatusCode;
            var statusDescription = HttpContext.Current.Response.StatusDescription;

            if (theError == null) theError = new HttpException(statusCode, statusDescription);

            var ctrlCtx = DirectViewRendererService.CreateController();

            var viewFindingErrorMessage = new StringBuilder();
            var errorViewNames = GetErrorViewNames(statusCode).ToArray();
            var errorViewFound = false;
            for (var i = 0; i < errorViewNames.Length && !errorViewFound; i++) {
                try {
                    var errorViewName = errorViewNames[i];
                    var response = ctrlCtx.RenderViewToString(errorViewName, theError);

                    res.TrySkipIisCustomErrors = true;
                    res.Clear();
                    res.ContentType = "text/html";
                    res.Write(response);
                    errorViewFound = true;
                } catch (FileNotFoundException notFoundExcp) {
                    viewFindingErrorMessage.Append(notFoundExcp.Message);
                }
            }

            if (!errorViewFound) {
                var excp = new FileNotFoundException(
                    $"{typeof(ErrorRazorHttpModule)} can't find error view template (please also check InnerException property of this exception).\r\n{viewFindingErrorMessage}",
                    theError
                );
                _logger.Error(excp);
                throw excp;
            }
        }

        private IEnumerable<string> GetErrorViewNames(int statusCode) {
            var tplNames = GetHttpStatusCodeTemplateNames(statusCode);
            foreach (var tplName in tplNames) yield return MyAppSettings.ErrorViewNamePrefix + tplName;
        }

        /// <summary>
        ///     From status code like 404 to a list (404,40x, 4xx, xxx)
        ///     312 -> 312, 31x, 3xx, xxx
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        private IEnumerable<string> GetHttpStatusCodeTemplateNames(int statusCode) {
            var statusCodeString = statusCode.ToString();
            yield return statusCodeString;
            for (var i = statusCodeString.Length - 1; i >= 0; i--) {
                var scs = statusCodeString.ToCharArray();
                for (var j = statusCodeString.Length - 1; j >= i; j--) scs[j] = 'x';
                yield return new string(scs);
            }
        }

        private bool IsHandledStatusCode(int statusCode) {
            var scs = statusCode.ToString();
            foreach (var hsc in MyAppSettings.HandledStatusCodes) {
                var hscChars = hsc.ToCharArray();
                var scsChars = scs.ToCharArray();

                if (hscChars.Length == scsChars.Length) {
                    var stillMatch = true;
                    for (var i = 0; i < hscChars.Length && stillMatch; i++) {
                        if (hscChars[i] != scsChars[i]) {
                            if (hscChars[i] != 'x' && hscChars[i] != 'X') stillMatch = false;
                        }
                    }
                    if (stillMatch) return true;
                }
            }
            return false;
        }
    }
}