using System;
using Newtonsoft.Json;
using JrzAsp.Lib.ProtoCms.Core.Models;

namespace JrzAsp.Lib.ProtoCms.Vue.Models {
    public class VueMenuItem : VueLink {

        private Func<ProtoCmsRuntimeContext, bool> _isVisible;
        public string Id { get; set; }
        public string CategoryId { get; set; }
        public decimal RowOrder { get; set; }

        [JsonIgnore]
        public Func<ProtoCmsRuntimeContext, bool> IsVisible {
            get {
                if (_isVisible == null) {
                    _isVisible = ctx => true;
                }
                return _isVisible;
            }
            set => _isVisible = value;
        }
    }
}