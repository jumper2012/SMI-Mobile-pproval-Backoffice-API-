using System;
using System.Collections.Generic;
using System.Linq;

namespace JrzAsp.Lib.ProtoCms.Datum.Models {
    public class CombinedDatumModifyOperation : DatumModifyOperation {
        public CombinedDatumModifyOperation(string name, IEnumerable<DatumModifyOperation> operations,
            IEnumerable<Type> providerTypes)
            : base(
                name,
                CombineDevDescriptions(operations),
                DetermineIsCreateLike(operations, name)
            ) {
            __ProviderTypes = providerTypes.ToArray();
        }

        public Type[] __ProviderTypes { get; }

        private static string CombineDevDescriptions(IEnumerable<DatumModifyOperation> operations) {
            return string.Join("; ", operations.ToArray().Select(x => x.DevDescription));
        }

        private static bool DetermineIsCreateLike(IEnumerable<DatumModifyOperation> operations, string name) {
            var ops = operations.ToArray();
            var ors = ops.Select(x => x.IsCreatingNewDatum).Aggregate((a, b) => a || b);
            var ands = ops.Select(x => x.IsCreatingNewDatum).Aggregate((a, b) => a && b);
            if (ors != ands) {
                throw new InvalidOperationException(
                    $"ProtoCMS: datum modify operation '{name}' {nameof(IsCreatingNewDatum)} can't be determined.");
            }
            return ors;
        }
    }
}