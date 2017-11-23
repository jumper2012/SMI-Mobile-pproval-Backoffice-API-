using System;
using System.Linq;
using System.Linq.Expressions;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.Trashing {
    public class TrashingFieldTableFilterHandler : IContentTableFilterHandler, IContentWhereHandler {

        public const string TRASHING_FILTER_ID = "trash-status-filter";

        public string[] HandledContentTypeIds => new[] {ContentType.ANY_CONTENT_TYPE_ID};
        public decimal Priority => 0;

        public string Id => TRASHING_FILTER_ID;
        public string Name => "Trash Status";
        public string Description => "Filter content by its trash status.";

        public bool CanFilter(ContentType contentType) {
            return true;
        }

        public ContentTableFilterForm BuildFilterForm(ContentType contentType) {
            return new TrashingFieldTableFilterForm();
        }

        public VueComponentDefinition[] FilterFormVues(ContentTableFilterForm filterForm, ContentType contentType) {
            return new[] {
                new VueComponentDefinition {
                    Name = "cms-form-field-checkbox",
                    Props = new {
                        label = "Trash Status:",
                        valuePath = nameof(TrashingFieldTableFilterForm.IsTrashed),
                        yesLabel = "Trashed",
                        noLabel = "Not Trashed"
                    }
                }
            };
        }

        public ContentTableFilterOperation[] SetupFilteringOperations(ContentTableFilterForm filterForm,
            ContentType contentType) {
            if (!(filterForm is TrashingFieldTableFilterForm)) return null;
            var f = filterForm.DirectCastTo<TrashingFieldTableFilterForm>();
            return new[] {
                new ContentTableFilterOperation(TrashingFieldWhereConditionsProvider.IS_TRASHED_WHERE_CONDITION_NAME, f)
            };
        }

        public Expression<Func<ProtoContent, bool>> HandleWhere(ContentWhereCondition condition, object param,
            ContentType contentType, out bool callNextHandler) {

            callNextHandler = true;

            if (!condition.IsTrashedCondition() && !(param is TrashingFieldTableFilterForm)) {
                return null;
            }
            var par = param.DirectCastTo<TrashingFieldTableFilterForm>();

            var tfn = $"{ContentType.FIELD_NAME_PUBLISH_STATUS}.{nameof(TrashingField.TrashedUtc)}";

            var now = DateTime.UtcNow;

            if (par.IsTrashed) {
                return x => (
                    from tf in x.ContentFields.Where(cf => cf.FieldName == tfn)
                    select new {
                        TrashedUtc = tf.DateTimeValue
                    }).Any(y => y.TrashedUtc != null && y.TrashedUtc.Value <= now);
            }
            return x => !(
                from tf in x.ContentFields.Where(cf => cf.FieldName == tfn)
                select new {
                    TrashedUtc = tf.DateTimeValue
                }).Any(y => y.TrashedUtc != null && y.TrashedUtc.Value <= now);
        }
    }
}