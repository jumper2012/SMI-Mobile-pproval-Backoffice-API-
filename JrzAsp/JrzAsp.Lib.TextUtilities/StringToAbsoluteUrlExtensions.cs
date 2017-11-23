﻿using System.Web;

namespace JrzAsp.Lib.TextUtilities {
    /// <summary>
    ///     Converts the provided app-relative path into an absolute Url containing the full host name
    /// </summary>
    public static class StringToAbsoluteUrlExtensions {
        /// <summary>
        ///     Converts the provided app-relative path into an absolute Url containing the
        ///     full host name
        /// </summary>
        /// <param name="relativeUrl">App-Relative path</param>
        /// <returns>Provided relativeUrl parameter as fully qualified Url</returns>
        /// <example>~/path/to/foo to http://www.web.com/path/to/foo</example>
        public static string ToAbsoluteUrl(this string relativeUrl) {
            if (string.IsNullOrEmpty(relativeUrl)) {
                return relativeUrl;
            }

            if (HttpContext.Current == null) {
                return relativeUrl;
            }

            if (relativeUrl.StartsWith("/")) {
                relativeUrl = relativeUrl.Insert(0, "~");
            }
            if (!relativeUrl.StartsWith("~/")) {
                relativeUrl = relativeUrl.Insert(0, "~/");
            }

            var url = HttpContext.Current.Request.Url;
            var port = url.Port != 80 ? ":" + url.Port : string.Empty;

            return $"{url.Scheme}://{url.Host}{port}{VirtualPathUtility.ToAbsolute(relativeUrl)}";
        }
    }
}