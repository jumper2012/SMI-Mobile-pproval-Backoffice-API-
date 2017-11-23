using System;
using JrzAsp.Lib.ProtoCms.Datum.Models;

namespace JrzAsp.Lib.ProtoCms.Datum.API {
    public class DatumHandlerApiInfo {
        public Type HandlerType { get; set; }
        public string HandledDatumType { get; set; }
        public string HandledType { get; set; }
        public decimal Priority { get; set; }
    }
}