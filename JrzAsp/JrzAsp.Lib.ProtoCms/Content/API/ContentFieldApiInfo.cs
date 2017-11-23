using System;
using JrzAsp.Lib.ProtoCms.Content.Models;

namespace JrzAsp.Lib.ProtoCms.Content.API {
    public class ContentFieldApiInfo {
        public Type ModelType { get; set; }
        public ContentFieldSpec FieldSpec { get; set; }
    }
}