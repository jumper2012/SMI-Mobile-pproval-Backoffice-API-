using System;
using System.Linq;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Datum.Services {
    public interface IDatumHandler : IPerRequestDependency {
        decimal Priority { get; }
        Type DatumModelType { get; }
    }
}