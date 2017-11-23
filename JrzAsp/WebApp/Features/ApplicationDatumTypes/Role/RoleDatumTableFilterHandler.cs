using System;
using System.Linq;
using System.Linq.Expressions;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using JrzAsp.Lib.ProtoCms.Role.Models;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.TypeUtilities;
using WebApp.Models;

namespace WebApp.Features.ApplicationDatumTypes.Role {
    public class RoleDatumTableFilterHandler : BaseDatumWhereHandler<ApplicationRole>, IDatumTableFilterHandler {

        public override decimal Priority => 0;

        public string Id => "role-system-reserved";
        public string Name => "System Reserved Role";
        public string Description => "Filter role according to its system reserved status.";

        public ContentTableFilterForm BuildFilterForm(Type datumType) {
            return new RoleDatumIsReservedTableFilterForm();
        }

        public VueComponentDefinition[] FilterFormVues(ContentTableFilterForm filterForm, Type datumType) {
            return new[] {
                new VueComponentDefinition {
                    Name = "cms-form-field-checkbox",
                    Props = new {
                        label = "Is System Reserved?",
                        valuePath = nameof(RoleDatumIsReservedTableFilterForm.IsReserved),
                        yesLabel = "Yes",
                        noLabel = "No"
                    }

                }
            };
        }

        public ContentTableFilterOperation[]
            SetupFilteringOperations(ContentTableFilterForm filterForm, Type datumType) {
            var f = filterForm is RoleDatumIsReservedTableFilterForm
                ? filterForm.DirectCastTo<RoleDatumIsReservedTableFilterForm>()
                : null;
            if (f == null) return null;
            return new[] {
                new ContentTableFilterOperation(
                    RoleDatumWhereConditionsProvider.IS_SYSTEM_RESERVED_WHERE_CONDITION_NAME, f
                )
            };
        }

        public override Expression<Func<ApplicationRole, bool>> HandleWhere(DatumWhereCondition condition, object param,
            Type datumType, out bool callNextHandler) {
            callNextHandler = true;
            if (condition.Is(RoleDatumWhereConditionsProvider.IS_SYSTEM_RESERVED_WHERE_CONDITION_NAME)
                && param is RoleDatumIsReservedTableFilterForm) {
                var par = param.DirectCastTo<RoleDatumIsReservedTableFilterForm>();
                var isRes = par.IsReserved;
                var reservedNames = new[] {
                    RequiredRoleInfo.Authenticated.Name, RequiredRoleInfo.Guest.Name, RequiredRoleInfo.SuperAdmin.Name
                };
                if (isRes) {
                    return x => reservedNames.Any(rn => rn.ToLower() == x.Name.ToLower());
                }
                return x => reservedNames.All(rn => rn.ToLower() != x.Name.ToLower());
            }
            return null;
        }
    }
}