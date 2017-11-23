using System;
using System.Linq.Expressions;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.TypeUtilities;
using WebApp.Models;

namespace WebApp.Features.ApplicationDatumTypes.User {
    public class UserDatumTableFilterHandler : BaseDatumWhereHandler<ApplicationUser>, IDatumTableFilterHandler {

        public override decimal Priority => 0;

        public string Id => "user-active-status";
        public string Name => "User Active Status";
        public string Description => "Filter user according to his/her active status.";

        public ContentTableFilterForm BuildFilterForm(Type datumType) {
            return new UserDatumIsActivatedTableFilterForm();
        }

        public VueComponentDefinition[] FilterFormVues(ContentTableFilterForm filterForm, Type datumType) {
            return new[] {
                new VueComponentDefinition {
                    Name = "cms-form-field-checkbox",
                    Props = new {
                        label = "Is Activated?",
                        valuePath = nameof(UserDatumIsActivatedTableFilterForm.IsActivated),
                        yesLabel = "Yes",
                        noLabel = "No"
                    }

                }
            };
        }

        public ContentTableFilterOperation[]
            SetupFilteringOperations(ContentTableFilterForm filterForm, Type datumType) {
            var f = filterForm is UserDatumIsActivatedTableFilterForm
                ? filterForm.DirectCastTo<UserDatumIsActivatedTableFilterForm>()
                : null;
            if (f == null) return null;
            return new[] {
                new ContentTableFilterOperation(
                    UserDatumWhereConditionsProvider.IS_ACTIVATED_WHERE_CONDITION_NAME, f
                )
            };
        }

        public override Expression<Func<ApplicationUser, bool>> HandleWhere(DatumWhereCondition condition, object param,
            Type datumType, out bool callNextHandler) {
            callNextHandler = true;
            if (condition.Is(UserDatumWhereConditionsProvider.IS_ACTIVATED_WHERE_CONDITION_NAME)
                && param is UserDatumIsActivatedTableFilterForm) {
                var par = param.DirectCastTo<UserDatumIsActivatedTableFilterForm>();
                var isActive = par.IsActivated;
                return x => x.IsActivated == isActive;
            }
            return null;
        }
    }
}