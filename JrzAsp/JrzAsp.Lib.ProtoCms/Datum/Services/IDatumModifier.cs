using System;
using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Datum.Permissions;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.RepoPattern;

namespace JrzAsp.Lib.ProtoCms.Datum.Services {
    public interface IDatumModifier : IAlwaysFreshDependency {
        Type DatumModelType { get; }
        DatumType DatumTypeBase { get; }
        IDatumModifierHandler[] HandlersBase { get; }
        IDatumPermissionsHandler[] PermissionsHandlersBase { get; }
        ModifyDatumPermission[] PermissionsToModifyBase { get; }
        
        object FindDatumBase(string datumId, string operationName);
        
        IDictionary<string, ContentModifierForm> BuildModifierForm(string datumId, string operationName);

        IDictionary<string, VueComponentDefinition[]> ConvertFormToVues(
            IDictionary<string, ContentModifierForm> modifierForm, string datumId, string operationName);

        FurtherValidationResult ValidateModifierForm(IDictionary<string, ContentModifierForm> modifierForm,
            string datumId, string operationName);

        void PerformModificationBase(IDictionary<string, ContentModifierForm> modifierForm, string datumId,
            out object modifiedDatum, string operationName);

        DatumModifyOperation FindModifyOperation(string operationName);
    }

    public interface IDatumModifier<TDat> : IDatumModifier {
        DatumType<TDat> DatumType { get; }
        IDatumModifierHandler<TDat>[] Handlers { get; }
        IDatumPermissionsHandler<TDat>[] PermissionsHandlers { get; }
        ModifyDatumPermission<TDat>[] PermissionsToModify { get; }
        
        TDat FindDatum(string datumId, string operationName);
        
        void PerformModification(IDictionary<string, ContentModifierForm> modifierForm, string datumId,
            out TDat modifiedDatum, string operationName);
        
    }
}