using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace JrzAsp.Lib.TextUtilities {
    public static class StringSplitAsKeywordsExtensions {
        public static IEnumerable<string> SplitAsKeywords(this string keywords) {
            if (string.IsNullOrWhiteSpace(keywords)) {
                return new string[0];
            }

            const string splitterPattern = @"\s+";
            return Regex.IsMatch(keywords, splitterPattern)
                ? Regex.Split(keywords, splitterPattern)
                : new[] {keywords};
        }
    }
}