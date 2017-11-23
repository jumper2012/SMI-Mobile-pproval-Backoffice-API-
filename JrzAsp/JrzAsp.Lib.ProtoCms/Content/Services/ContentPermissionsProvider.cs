using System;
using System.Collections.Generic;
using System.Linq;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Permissions;
using JrzAsp.Lib.ProtoCms.Permission.Models;
using JrzAsp.Lib.ProtoCms.Permission.Services;

namespace JrzAsp.Lib.ProtoCms.Content.Services {
    public class ContentPermissionsProvider : IPermissionsProvider {

        public IEnumerable<ProtoPermission> DefinePermissions() {
            foreach (var ct in ContentType.DefinedTypes) {
                yield return new ViewContentPermission(ct);
                yield return new ListContentPermission(ct);
                var modDups = new Dictionary<string, int>();
                foreach (var mo in ContentModifier.DefinedModifyOperations) {
                    if (ct.DisabledModifyOperationNames.Contains(mo.Name)) continue;

                    if (!modDups.TryGetValue(mo.Name, out var count)) {
                        count = 0;
                    }
                    count++;
                    if (count > 1) {
                        throw new InvalidOperationException(
                            $"ProtoCMS: content modify operation '{mo.Name}' is defined more than once."
                        );
                    }
                    modDups[mo.Name] = count;
                    yield return mo.BuildPermission(ct);
                }
            }
        }
    }
}