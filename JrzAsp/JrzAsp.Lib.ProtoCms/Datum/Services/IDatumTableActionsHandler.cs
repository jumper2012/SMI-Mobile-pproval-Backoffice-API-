using System;
using JrzAsp.Lib.ProtoCms.Core.Models;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Datum.Services {
    public interface IDatumTableActionsHandler : IDatumHandler {
        VueActionTrigger[] TableActionsForNoContent(ProtoCmsRuntimeContext cmsContext, Type datumType);

        VueActionTrigger[] TableActionsForSingleContentBase(object datum, ProtoCmsRuntimeContext cmsContext,
            Type datumType);
    }

    public interface IDatumTableActionsHandler<TDat> : IDatumTableActionsHandler {
        VueActionTrigger[] TableActionsForSingleContent(TDat datum, ProtoCmsRuntimeContext cmsContext, Type datumType);
    }

    public abstract class BaseDatumTableActionsHandler<TDat> : IDatumTableActionsHandler<TDat> {

        public abstract decimal Priority { get; }
        public Type DatumModelType => typeof(TDat);

        public abstract VueActionTrigger[] TableActionsForSingleContent(TDat datum, ProtoCmsRuntimeContext cmsContext,
            Type datumType);

        public abstract VueActionTrigger[] TableActionsForNoContent(ProtoCmsRuntimeContext cmsContext, Type datumType);

        public VueActionTrigger[] TableActionsForSingleContentBase(object datum, ProtoCmsRuntimeContext cmsContext,
            Type datumType) {
            return TableActionsForSingleContent(datum.DirectCastTo<TDat>(), cmsContext, datumType);
        }
    }
}