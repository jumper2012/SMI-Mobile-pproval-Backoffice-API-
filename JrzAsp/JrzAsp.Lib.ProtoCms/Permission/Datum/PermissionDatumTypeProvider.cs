using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using JrzAsp.Lib.ProtoCms.Permission.Models;

namespace JrzAsp.Lib.ProtoCms.Permission.Datum {
    public class PermissionDatumTypeProvider : IDatumTypesProvider {
        public IEnumerable<DatumType> DefineDatumTypeInfos() {
            yield return new DatumType<ProtoPermission>(
                "permission",
                "Role Permissions",
                "Permission to control website access given to user role.",
                new [] {
                    nameof(ProtoPermission.CategoryName),
                    nameof(ProtoPermission.SubCategoryName),
                    nameof(ProtoPermission.DisplayName)
                },
                new [] {
                    nameof(ProtoPermission.CategoryName),
                    nameof(ProtoPermission.SubCategoryName),
                    nameof(ProtoPermission.DisplayName),
                    nameof(ProtoPermission.Description),
                    nameof(ProtoPermission.Id),
                },
                "Category",
                false,
                new[] {"*"},
                null
            );
        }
    }
}