using System;

namespace JrzAsp.Lib.ProtoCms.Content.API {
    public class ContentHandlerApiInfo {
        public Type HandlerType { get; set; }
        public string[] HandledContentTypes { get; set; }
        public decimal Priority { get; set; }
    }
}