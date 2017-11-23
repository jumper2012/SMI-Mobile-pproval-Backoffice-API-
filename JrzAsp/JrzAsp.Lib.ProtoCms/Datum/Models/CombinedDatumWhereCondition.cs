using System;
using System.Collections.Generic;
using System.Linq;

namespace JrzAsp.Lib.ProtoCms.Datum.Models {
    public class CombinedDatumWhereCondition : DatumWhereCondition {
        public CombinedDatumWhereCondition(string name, IEnumerable<DatumWhereCondition> conditions,
            IEnumerable<Type> providerTypes) :
            base(name, string.Join("; ", conditions.Select(x => x.DevDescription))) {
            __ProviderTypes = providerTypes.ToArray();
        }

        public Type[] __ProviderTypes { get; }
    }
}