using System;
using System.Linq;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Datum.Services {
    public interface IDatumGetterHandler : IDatumHandler {
        string GetDatumIdBase(object datum);
        IQueryable BuildBaseQueryBase();
        object CreateInMemoryBase(string datumId);
        object FindByIdBase(string id);
    }

    public interface IDatumGetterHandler<TDat> : IDatumGetterHandler {
        string GetDatumId(TDat datum);
        IQueryable<TDat> BuildBaseQuery();
        TDat CreateInMemory(string datumId);
        TDat FindById(string id);
    }

    public abstract class BaseDatumGetterHandler<TDat> : IDatumGetterHandler<TDat> {

        public abstract decimal Priority { get; }
        public Type DatumModelType => typeof(TDat);

        public string GetDatumIdBase(object datum) {
            return GetDatumId(datum.DirectCastTo<TDat>());
        }

        public IQueryable BuildBaseQueryBase() {
            return BuildBaseQuery();
        }

        public object CreateInMemoryBase(string datumId) {
            return CreateInMemory(datumId);
        }

        public object FindByIdBase(string id) {
            return FindById(id);
        }
        
        public abstract string GetDatumId(TDat datum);
        public abstract IQueryable<TDat> BuildBaseQuery();
        public abstract TDat CreateInMemory(string datumId);
        public abstract TDat FindById(string id);
    }
}