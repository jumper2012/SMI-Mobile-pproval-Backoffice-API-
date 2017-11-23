using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Datum.Models;

namespace JrzAsp.Lib.ProtoCms.Datum.Services {
    public interface IDatumTypesProvider : IGlobalSingletonDependency {
        IEnumerable<DatumType> DefineDatumTypeInfos();
    }
}