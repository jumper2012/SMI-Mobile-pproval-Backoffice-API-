namespace JrzAsp.Lib.ProtoCms.Vue.Models {
    public class VueButton : VueActionTrigger {
        private string _buttonHtmlType;
        public string ButtonHtmlType {
            get {
                if (string.IsNullOrWhiteSpace(_buttonHtmlType)) {
                    _buttonHtmlType = "button";
                }
                return _buttonHtmlType;
            }
            set { _buttonHtmlType = value; }
        }
        public string OnClick { get; set; }
    }
}