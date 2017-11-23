using System.Linq;
using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using JrzAsp.Lib.ProtoCms.Permission.Models;

namespace JrzAsp.Lib.ProtoCms.Datum.Permissions {
    public abstract class ModifyDatumPermission : ProtoPermission {
        protected ModifyDatumPermission(string modifyOperationName) {
            ModifyOperationName = modifyOperationName;
        }

        public string ModifyOperationName { get; }
        public abstract DatumType DatumTypeBase { get; }

        public override bool AuthenticatedUserHasThisByDefault => false;
        public override bool GuestUserHasThisByDefault => false;

        public static string GetIdFor(DatumType datumType, string modifyOperationName) {
            return
                $"Datum({datumType.Id}):Modify({modifyOperationName})";
        }
    }

    public class ModifyDatumPermission<TDat> : ModifyDatumPermission {
        private readonly IDatumPermissionsHandler<TDat> _handler;

        public ModifyDatumPermission(string modifyOperationName, IDatumPermissionsHandler<TDat> handler) : base(
            modifyOperationName) {
            _handler = handler;
        }

        public DatumType<TDat> DatumType => typeof(TDat).GetDatumTypeFromType<TDat>();

        private DatumPermissionCustomProps CustomProps =>
            _handler.ModifyPermissionCustomProps(
                DatumModifier<TDat>.DefinedModifyOperations.First(x => x.Name == ModifyOperationName), DatumType);

        public override string Id => GetIdFor(DatumType, ModifyOperationName);
        public override string DisplayName => CustomProps?.DisplayName;
        public override string Description => CustomProps?.Description;
        public override string CategoryName => CustomProps?.CategoryName;
        public override string SubCategoryName => CustomProps?.SubCategoryName;

        public override DatumType DatumTypeBase => DatumType;

        public static string GetIdFor(DatumType<TDat> datumType, string modifyOperationName) {
            return
                $"Datum({datumType.Id}):Modify({modifyOperationName})";
        }
    }
}