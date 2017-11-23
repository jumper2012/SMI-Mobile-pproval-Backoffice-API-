using System;
using System.Linq.Expressions;

namespace JrzAsp.Lib.ProtoCms.Datum.Services {
    public interface IDatumSearchHandler : IDatumHandler {
        Expression HandleSearchBase(string keywords, string[] splittedKeywords, Type datumType,
            out bool callNextHandler);
    }

    public interface IDatumSearchHandler<TDat> : IDatumSearchHandler {
        Expression<Func<TDat, bool>> HandleSearch(string keywords, string[] splittedKeywords, Type datumType,
            out bool callNextHandler);
    }

    public abstract class BaseDatumSearchHandler<TDat> : IDatumSearchHandler<TDat> {
        public abstract decimal Priority { get; }
        public Type DatumModelType => typeof(TDat);

        public Expression HandleSearchBase(string keywords, string[] splittedKeywords, Type datumType,
            out bool callNextHandler) {
            return HandleSearch(keywords, splittedKeywords, datumType, out callNextHandler);
        }
        public abstract Expression<Func<TDat, bool>> HandleSearch(string keywords, string[] splittedKeywords, Type datumType, out bool callNextHandler);
    }
}