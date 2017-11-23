using HtmlAgilityPack;

namespace JrzAsp.Lib.TextUtilities {
    public static class StringStripHtmlTagsExtensions {
        public static string StripHtmlTags(this string text) {
            if (string.IsNullOrWhiteSpace(text)) return text;
            var doc = new HtmlDocument();
            doc.LoadHtml(text);
            return doc.DocumentNode.InnerText;
        }
    }
}