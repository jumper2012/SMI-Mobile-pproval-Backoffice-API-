using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using JrzAsp.Lib.ProtoCms.Permission.Models;

namespace JrzAsp.Lib.ProtoCms.Datum.Permissions {
    public abstract class ViewDatumPermission : ProtoPermission {

        public abstract DatumType DatumTypeBase { get; }
        public override bool AuthenticatedUserHasThisByDefault => true;
        public override bool GuestUserHasThisByDefault => true;

        public static string GetIdFor(DatumType datumType) {
            return $"Datum({datumType.Id}):View";
        }
    }

    public class ViewDatumPermission<TDat> : ViewDatumPermission {
        private readonly IDatumPermissionsHandler<TDat> _handler;

        public ViewDatumPermission(IDatumPermissionsHandler<TDat> handler) {
            _handler = handler;
        }

        public DatumType<TDat> DatumType => typeof(TDat).GetDatumTypeFromType<TDat>();
        private DatumPermissionCustomProps CustomProps => _handler.ViewPermissionCustomProps(DatumType);

        public override string Id => GetIdFor(DatumType);
        public override string DisplayName => CustomProps.DisplayName;
        public override string Description => CustomProps.Description;
        public override string CategoryName => CustomProps.CategoryName;
        public override string SubCategoryName => CustomProps.SubCategoryName;
        public override DatumType DatumTypeBase => DatumType;

        public static string GetIdFor(DatumType<TDat> datumType) {
            return $"Datum({datumType.Id}):View";
        }
    }
}