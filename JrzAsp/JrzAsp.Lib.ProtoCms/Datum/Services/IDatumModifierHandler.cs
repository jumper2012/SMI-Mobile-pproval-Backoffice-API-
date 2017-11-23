using System;
using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.RepoPattern;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Datum.Services {
    public interface IDatumModifierHandler : IDatumHandler {
        IDictionary<string, ContentModifierForm> BuildModifierFormBase(object datum,
            DatumModifyOperation operation, Type datumType);

        IDictionary<string, VueComponentDefinition[]> ConvertFormToVuesBase(
            IDictionary<string, ContentModifierForm> modifierForm,
            object datum, DatumModifyOperation operation, Type datumType);

        FurtherValidationResult ValidateModifierFormBase(IDictionary<string, ContentModifierForm> modifierForm,
            object datum, DatumModifyOperation operation, Type datumType);

        void PerformModificationBase(IDictionary<string, ContentModifierForm> modifierForm, object datum,
            DatumModifyOperation operation, Type datumType);
    }

    public interface IDatumModifierHandler<TDat> : IDatumModifierHandler {
        IDictionary<string, ContentModifierForm> BuildModifierForm(TDat datum,
            DatumModifyOperation operation, Type datumType);

        IDictionary<string, VueComponentDefinition[]> ConvertFormToVues(
            IDictionary<string, ContentModifierForm> modifierForm,
            TDat datum, DatumModifyOperation operation, Type datumType);

        FurtherValidationResult ValidateModifierForm(IDictionary<string, ContentModifierForm> modifierForm,
            TDat datum, DatumModifyOperation operation, Type datumType);

        void PerformModification(IDictionary<string, ContentModifierForm> modifierForm, TDat datum,
            DatumModifyOperation operation, Type datumType);
    }

    public abstract class BaseDatumModifierHandler<TDat> : IDatumModifierHandler<TDat> {

        public abstract decimal Priority { get; }
        public Type DatumModelType => typeof(TDat);

        public IDictionary<string, ContentModifierForm> BuildModifierFormBase(object datum,
            DatumModifyOperation operation, Type datumType) {
            return BuildModifierForm(datum.DirectCastTo<TDat>(), operation, datumType);
        }

        public IDictionary<string, VueComponentDefinition[]> ConvertFormToVuesBase(
            IDictionary<string, ContentModifierForm> modifierForm, object datum, DatumModifyOperation operation,
            Type datumType) {
            return ConvertFormToVues(modifierForm, datum.DirectCastTo<TDat>(), operation, datumType);
        }

        public FurtherValidationResult ValidateModifierFormBase(IDictionary<string, ContentModifierForm> modifierForm,
            object datum, DatumModifyOperation operation,
            Type datumType) {
            return ValidateModifierForm(modifierForm, datum.DirectCastTo<TDat>(), operation, datumType);
        }

        public void PerformModificationBase(IDictionary<string, ContentModifierForm> modifierForm, object datum,
            DatumModifyOperation operation, Type datumType) {
            PerformModification(modifierForm, datum.DirectCastTo<TDat>(), operation, datumType);
        }

        public abstract IDictionary<string, ContentModifierForm> BuildModifierForm(TDat datum,
            DatumModifyOperation operation, Type datumType);

        public abstract IDictionary<string, VueComponentDefinition[]> ConvertFormToVues(
            IDictionary<string, ContentModifierForm> modifierForm, TDat datum, DatumModifyOperation operation,
            Type datumType);

        public abstract FurtherValidationResult ValidateModifierForm(
            IDictionary<string, ContentModifierForm> modifierForm, TDat datum, DatumModifyOperation operation,
            Type datumType);

        public abstract void PerformModification(IDictionary<string, ContentModifierForm> modifierForm, TDat datum,
            DatumModifyOperation operation, Type datumType);
    }
}