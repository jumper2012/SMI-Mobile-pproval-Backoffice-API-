using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using JrzAsp.Lib.ProtoCms.Permission.Models;

namespace JrzAsp.Lib.ProtoCms.Datum.Permissions {
    public abstract class ListDatumPermission : ProtoPermission {

        public abstract DatumType DatumTypeBase { get; }

        public override bool AuthenticatedUserHasThisByDefault => false;
        public override bool GuestUserHasThisByDefault => false;

        public static string GetIdFor(DatumType datumType) {
            return $"Datum({datumType.Id}):List";
        }
    }

    public class ListDatumPermission<TDat> : ListDatumPermission {
        private readonly IDatumPermissionsHandler<TDat> _handler;

        public ListDatumPermission(IDatumPermissionsHandler<TDat> handler) {
            _handler = handler;
        }

        public DatumType<TDat> DatumType => typeof(TDat).GetDatumTypeFromType<TDat>();
        private DatumPermissionCustomProps CustomProps => _handler.ListPermissionCustomProps(DatumType);

        public override string Id => GetIdFor(DatumType);
        public override string DisplayName => CustomProps.DisplayName;
        public override string Description => CustomProps.Description;
        public override string CategoryName => CustomProps.CategoryName;
        public override string SubCategoryName => CustomProps.SubCategoryName;

        public override DatumType DatumTypeBase => DatumType;

        public static string GetIdFor(DatumType<TDat> datumType) {
            return $"Datum({datumType.Id}):List";
        }
    }
}