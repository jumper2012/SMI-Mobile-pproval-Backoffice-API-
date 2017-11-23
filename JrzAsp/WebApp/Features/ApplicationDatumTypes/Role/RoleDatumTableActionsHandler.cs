using System;
using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Core.Models;
using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using WebApp.Models;

namespace WebApp.Features.ApplicationDatumTypes.Role {
    public class RoleDatumTableActionsHandler : BaseDatumTableActionsHandler<ApplicationRole> {

        public override decimal Priority => 0;

        public override VueActionTrigger[] TableActionsForSingleContent(ApplicationRole datum,
            ProtoCmsRuntimeContext cmsContext,
            Type datumType) {
            var dtt = datumType.GetDatumTypeFromType<ApplicationRole>();
            if (dtt == null) return null;

            var trigs = new List<VueActionTrigger>();
            if (dtt.IsModifyOperationAllowed(StandardModifyOperationsProvider.UPDATE_OPERATION_NAME)) {
                trigs.Add(
                    new VueButton {
                        Label = $"Update",
                        IconCssClass = "fa fa-pencil",
                        OnClick = $"protoCms.utils.popupEntityOperationForm('datum', '{datum.Id}', '{dtt.Id}', " +
                                  $"'{StandardModifyOperationsProvider.UPDATE_OPERATION_NAME}', " +
                                  $"'Update {datum.Name}', '{dtt.Name}')"
                    });
            }
            if (dtt.IsModifyOperationAllowed(StandardModifyOperationsProvider.DELETE_OPERATION_NAME)) {
                trigs.Add(
                    new VueButton {
                        Label = $"Delete",
                        IconCssClass = "fa fa-trash",
                        OnClick = $"protoCms.utils.popupEntityOperationForm('datum', '{datum.Id}', '{dtt.Id}', " +
                                  $"'{StandardModifyOperationsProvider.DELETE_OPERATION_NAME}', " +
                                  $"'Delete {datum.Name}', '{dtt.Name}')"
                    });
            }

            return trigs.ToArray();
        }

        public override VueActionTrigger[] TableActionsForNoContent(ProtoCmsRuntimeContext cmsContext, Type datumType) {
            var dtt = datumType.GetDatumTypeFromType<ApplicationRole>();
            if (dtt == null || !dtt.IsModifyOperationAllowed(StandardModifyOperationsProvider.CREATE_OPERATION_NAME)) {
                return null;
            }
            return new VueActionTrigger[] {
                new VueButton {
                    Label = $"Create New {dtt.Name}",
                    IconCssClass = "fa fa-plus",
                    OnClick = $"protoCms.utils.popupEntityOperationForm('datum', null, '{dtt.Id}', " +
                              $"'{StandardModifyOperationsProvider.CREATE_OPERATION_NAME}', " +
                              $"'Create {dtt.Name}', null)"
                }
            };
        }
    }
}