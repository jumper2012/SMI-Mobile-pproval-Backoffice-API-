using System.Text.RegularExpressions;

namespace JrzAsp.Lib.TextUtilities {
    public static class StringToUrlSlugExtensions {
        /// <summary>
        ///     See http://stackoverflow.com/a/2161706
        /// </summary>
        /// <param name="str">the string to be url slugged</param>
        /// <returns>url slug version of str</returns>
        public static string ToUrlSlug(this string str) {
            return str == null ? null : Regex.Replace(str, @"[^A-Za-z0-9_]+", "-");
        }
    }
}