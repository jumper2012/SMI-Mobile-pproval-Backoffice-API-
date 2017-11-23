using System;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Datum.Services {
    public interface IDatumViewerHandler : IDatumHandler {
        ContentTableHeader GetTableHeader(string fieldName);
        string GetSummarizedValueBase(object datum, string fieldName);
        VueComponentDefinition[] GetTableRowVueBase(object datum, string fieldName);
        ContentPreviewPart GetFullPreviewBase(object datum, string fieldName);
        string[] GetValidFieldNames();
    }

    public interface IDatumViewerHandler<TDat> : IDatumViewerHandler {
        string GetSummarizedValue(TDat datum, string fieldName);
        VueComponentDefinition[] GetTableRowVue(TDat datum, string fieldName);
        ContentPreviewPart GetFullPreview(TDat datum, string fieldName);
    }

    public abstract class BaseDatumViewerHandler<TDat> : IDatumViewerHandler<TDat> {

        public abstract decimal Priority { get; }
        public Type DatumModelType => typeof(TDat);

        public string GetSummarizedValueBase(object datum, string fieldName) {
            return GetSummarizedValue(datum.DirectCastTo<TDat>(), fieldName);
        }

        public VueComponentDefinition[] GetTableRowVueBase(object datum, string fieldName) {
            return GetTableRowVue(datum.DirectCastTo<TDat>(), fieldName);
        }

        public ContentPreviewPart GetFullPreviewBase(object datum, string fieldName) {
            return GetFullPreview(datum.DirectCastTo<TDat>(), fieldName);
        }

        public abstract string[] GetValidFieldNames();
        public abstract ContentTableHeader GetTableHeader(string fieldName);
        public abstract string GetSummarizedValue(TDat datum, string fieldName);
        public abstract VueComponentDefinition[] GetTableRowVue(TDat datum, string fieldName);
        public abstract ContentPreviewPart GetFullPreview(TDat datum, string fieldName);
    }
}