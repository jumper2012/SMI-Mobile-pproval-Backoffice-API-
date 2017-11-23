using System;
using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Datum.Permissions;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Datum.Services {
    public interface IDatumPermissionsHandler : IDatumHandler {
        DatumPermissionCustomProps ViewPermissionCustomPropsBase(DatumType datumType);
        DatumPermissionCustomProps ListPermissionCustomPropsBase(DatumType datumType);

        DatumPermissionCustomProps ModifyPermissionCustomPropsBase(DatumModifyOperation modifyOperation,
            DatumType datumType);
    }

    public interface IDatumPermissionsHandler<TDat> : IDatumPermissionsHandler {
        DatumPermissionCustomProps ViewPermissionCustomProps(DatumType<TDat> datumType);
        DatumPermissionCustomProps ListPermissionCustomProps(DatumType<TDat> datumType);

        DatumPermissionCustomProps ModifyPermissionCustomProps(DatumModifyOperation modifyOperation,
            DatumType<TDat> datumType);
    }

    public abstract class BaseDatumPermissionsHandler<TDat> : IDatumPermissionsHandler<TDat> {

        public abstract decimal Priority { get; }
        public Type DatumModelType => typeof(TDat);

        public DatumPermissionCustomProps ViewPermissionCustomPropsBase(DatumType datumType) {
            return ViewPermissionCustomProps(datumType.DirectCastTo<DatumType<TDat>>());
        }

        public DatumPermissionCustomProps ListPermissionCustomPropsBase(DatumType datumType) {
            return ListPermissionCustomProps(datumType.DirectCastTo<DatumType<TDat>>());
        }

        public DatumPermissionCustomProps ModifyPermissionCustomPropsBase(DatumModifyOperation modifyOperation,
            DatumType datumType) {
            return ModifyPermissionCustomProps(modifyOperation, datumType.DirectCastTo<DatumType<TDat>>());
        }

        public abstract DatumPermissionCustomProps ViewPermissionCustomProps(DatumType<TDat> datumType);
        public abstract DatumPermissionCustomProps ListPermissionCustomProps(DatumType<TDat> datumType);

        public abstract DatumPermissionCustomProps ModifyPermissionCustomProps(DatumModifyOperation modifyOperation,
            DatumType<TDat> datumType);
    }
}