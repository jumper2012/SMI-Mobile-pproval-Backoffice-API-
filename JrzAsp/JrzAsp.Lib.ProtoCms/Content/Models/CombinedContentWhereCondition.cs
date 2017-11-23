using System;
using System.Collections.Generic;
using System.Linq;

namespace JrzAsp.Lib.ProtoCms.Content.Models {
    public class CombinedContentWhereCondition : ContentWhereCondition {
        public CombinedContentWhereCondition(string name, IEnumerable<ContentWhereCondition> conditions,
            IEnumerable<Type> providerTypes) :
            base(name, string.Join("; ", conditions.Select(x => x.DevDescription))) {
            __ProviderTypes = providerTypes.ToArray();
        }

        public Type[] __ProviderTypes { get; }
    }
}