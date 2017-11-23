namespace JrzAsp.Lib.ProtoCms.Vue.Models {
    public class VueHtmlWidget : VueComponentDefinition {
        public VueHtmlWidget(string innerHtml, string cssClass = null, string cssStyle = null) {
            Name = "cms-widget-html";
            Props = new {
                innerHtml,
                cssClass,
                cssStyle
            };
        }
    }
}