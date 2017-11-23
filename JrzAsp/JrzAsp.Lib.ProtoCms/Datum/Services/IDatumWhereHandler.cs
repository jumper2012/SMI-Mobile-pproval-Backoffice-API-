using System;
using System.Linq.Expressions;
using JrzAsp.Lib.ProtoCms.Datum.Models;

namespace JrzAsp.Lib.ProtoCms.Datum.Services {
    public interface IDatumWhereHandler : IDatumHandler {
        Expression HandleWhereBase(DatumWhereCondition condition,
            object param, Type datumType, out bool callNextHandler);
    }

    public interface IDatumWhereHandler<TDat> : IDatumWhereHandler {
        Expression<Func<TDat, bool>> HandleWhere(DatumWhereCondition condition,
            object param, Type datumType, out bool callNextHandler);
    }

    public abstract class BaseDatumWhereHandler<TDat> : IDatumWhereHandler<TDat> {

        public abstract decimal Priority { get; }
        public Type DatumModelType => typeof(TDat);

        public abstract Expression<Func<TDat, bool>> HandleWhere(DatumWhereCondition condition, object param,
            Type datumType, out bool callNextHandler);

        public Expression HandleWhereBase(DatumWhereCondition condition, object param, Type datumType,
            out bool callNextHandler) {
            return HandleWhere(condition, param, datumType, out callNextHandler);
        }
    }
}