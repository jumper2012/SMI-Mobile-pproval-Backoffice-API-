namespace JrzAsp.Lib.ProtoCms.Vue.Models {
    public class VueComponentDefinition {
        private string _name;
        private dynamic _props;
        public string Name {
            get => _name;
            set => _name = value ?? "";
        }
        public dynamic Props {
            get {
                if (_props != null) return _props;
                _props = new object();
                return _props;
            }
            set => _props = value;
        }
    }
}