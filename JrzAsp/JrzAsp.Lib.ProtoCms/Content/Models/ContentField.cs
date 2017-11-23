using Newtonsoft.Json;

namespace JrzAsp.Lib.ProtoCms.Content.Models {
    public abstract class ContentField {
        [JsonIgnore]
        public abstract ContentFieldSpec __FieldSpec { get; }
    }
}