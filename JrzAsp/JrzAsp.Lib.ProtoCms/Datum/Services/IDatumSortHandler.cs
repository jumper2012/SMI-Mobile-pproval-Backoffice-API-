using System;
using System.Linq;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Datum.Services {
    public interface IDatumSortHandler : IDatumHandler {
        IQueryable HandleSortBase(IQueryable currentQuery,
            string fieldName,
            bool descending,
            Type datumType,
            out bool callNextHandler);
    }

    public interface IDatumSortHandler<TDat> : IDatumSortHandler {
        IQueryable<TDat> HandleSort(IQueryable<TDat> currentQuery,
            string fieldName,
            bool descending,
            Type datumType,
            out bool callNextHandler);
    }

    public abstract class BaseDatumSortHandler<TDat> : IDatumSortHandler<TDat> {

        public abstract decimal Priority { get; }
        public Type DatumModelType => typeof(TDat);

        public IQueryable HandleSortBase(IQueryable currentQuery, string fieldName, bool descending,
            Type datumType,
            out bool callNextHandler) {
            return HandleSort(currentQuery.DirectCastTo<IQueryable<TDat>>(), fieldName, descending, datumType,
                out callNextHandler);
        }

        public abstract IQueryable<TDat> HandleSort(IQueryable<TDat> currentQuery, string fieldName, bool descending,
            Type datumType,
            out bool callNextHandler);
    }
}