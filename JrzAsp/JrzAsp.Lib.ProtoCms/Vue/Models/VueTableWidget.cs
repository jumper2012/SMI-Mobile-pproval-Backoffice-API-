using System.Collections.Generic;
using System.Linq;

namespace JrzAsp.Lib.ProtoCms.Vue.Models {
    public class VueTableWidget : VueComponentDefinition {
        public VueTableWidget(string[] header, IEnumerable<string[]> body) {
            var tbody = body.ToArray();
            Name = "cms-widget-table";
            Props = new {
                thead = header,
                tbody
            };
        }
    }
}