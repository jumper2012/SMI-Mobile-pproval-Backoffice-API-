using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace JrzAsp.Lib.ProtoCms.WebApi.Cors {
    public class FilePathCorsEnablerHttpModule : IHttpModule {
        public static string AccessControlAllowOrigin { get; set; } = "*";
        public static ICollection<string> AllowedPathRegexPatterns { get; } = new List<string>();
        private static IDictionary<int, Regex> RegexCache { get; } = new ConcurrentDictionary<int, Regex>();

        public void Init(HttpApplication context) {
            context.BeginRequest += Context_BeginRequest;
        }

        public void Dispose() {
            // none
        }

        private void Context_BeginRequest(object sender, EventArgs e) {
            var app = (HttpApplication) sender;
            var ctx = app.Context;
            var filePath = ctx.Request.FilePath;

            for (var i = 0; i < AllowedPathRegexPatterns.Count; i++) {
                var regex = GetRegexFromCache(i);
                if (regex.IsMatch(filePath)) {
                    ctx.Response.Headers["Access-Control-Allow-Origin"] = AccessControlAllowOrigin;
                    return;
                }
            }
        }

        private static Regex GetRegexFromCache(int index) {
            if (index >= AllowedPathRegexPatterns.Count || index < 0) return null;

            Regex regex = null;
            if (RegexCache.TryGetValue(index, out regex)) return regex;

            var pattern = AllowedPathRegexPatterns.ElementAt(index);
            regex = new Regex(pattern, RegexOptions.CultureInvariant);
            RegexCache[index] = regex;

            return regex;
        }
    }
}