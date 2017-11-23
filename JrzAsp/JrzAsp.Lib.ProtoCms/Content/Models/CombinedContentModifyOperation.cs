using System;
using System.Collections.Generic;
using System.Linq;

namespace JrzAsp.Lib.ProtoCms.Content.Models {
    public class CombinedContentModifyOperation : ContentModifyOperation {
        public CombinedContentModifyOperation(string name, IEnumerable<ContentModifyOperation> operations,
            IEnumerable<Type> providerTypes) :
            base(name,
                CombineDevDescription(operations),
                type => { return string.Join(" | ", operations.Select(x => x.GeneratePermissionDisplayName(type))); },
                type => { return string.Join("; ", operations.Select(x => x.GeneratePermissionDescription(type))); },
                DetermineIsCreateLike(operations, name)
            ) {
            __ProviderTypes = providerTypes.ToArray();
        }

        public Type[] __ProviderTypes { get; }

        private static string CombineDevDescription(IEnumerable<ContentModifyOperation> operations) {
            return string.Join("; ", operations.Select(x => x.DevDescription));
        }

        private static bool DetermineIsCreateLike(IEnumerable<ContentModifyOperation> operations, string name) {
            var ops = operations.ToArray();
            var ors = ops.Select(x => x.IsCreatingNewContent).Aggregate((a, b) => a || b);
            var ands = ops.Select(x => x.IsCreatingNewContent).Aggregate((a, b) => a && b);
            if (ors != ands) {
                throw new InvalidOperationException(
                    $"ProtoCMS: content modify operation '{name}' {nameof(IsCreatingNewContent)} " +
                    $"can't be determined.");
            }
            return ors;
        }
    }
}