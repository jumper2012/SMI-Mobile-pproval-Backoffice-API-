using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Content.Models;

namespace JrzAsp.Lib.ProtoCms.Content.Services {
    public interface IContentTypesProvider : IGlobalSingletonDependency {
        IEnumerable<ContentType> DefineContentTypes();
    }
}