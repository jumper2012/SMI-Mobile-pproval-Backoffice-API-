using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;

namespace JrzAsp.Lib.ProtoCms {
    public static class Jsonizer {

        public static JsonSerializerSettings SerializerSettings => new JsonSerializerSettings {
            TypeNameHandling = TypeNameHandling.Auto,
            TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public static string Convert(object obj) {
            return JsonConvert.SerializeObject(obj, SerializerSettings);
        }

        public static TObj Parse<TObj>(string json) {
            return JsonConvert.DeserializeObject<TObj>(json, SerializerSettings);
        }
    }
}