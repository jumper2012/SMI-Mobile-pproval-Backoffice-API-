using System;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;

namespace JrzAsp.Lib.ProtoCms.WebApi.Json {
    public static class DefaultJsonSerializerSettingsProvider {
        public static Func<JsonSerializerSettings> GetSerializerSettings { get; set; }
            = () => new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
            };
    }
}