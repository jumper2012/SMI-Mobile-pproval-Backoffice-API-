using System;
using System.Linq;
using System.Linq.Expressions;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.Publishing {
    public class PublishingFieldTableFilterHandler : IContentTableFilterHandler, IContentWhereHandler {
        public const string PUBLISHING_FILTER_ID = "publish-status-filter";

        public string[] HandledContentTypeIds => new[] {ContentType.ANY_CONTENT_TYPE_ID};
        public decimal Priority => 0;

        public string Id => PUBLISHING_FILTER_ID;
        public string Name => "Publish Status";
        public string Description => "Filter content by its publish status.";

        public bool CanFilter(ContentType contentType) {
            return true;
        }

        public ContentTableFilterForm BuildFilterForm(ContentType contentType) {
            return new PublishingFieldTableFilterForm();
        }

        public VueComponentDefinition[] FilterFormVues(ContentTableFilterForm filterForm, ContentType contentType) {
            return new[] {
                new VueComponentDefinition {
                    Name = "cms-form-field-checkbox",
                    Props = new {
                        label = "Publish Status:",
                        valuePath = nameof(PublishingFieldTableFilterForm.IsPublished),
                        yesLabel = "Published",
                        noLabel = "Not Published"
                    }
                }
            };
        }

        public ContentTableFilterOperation[] SetupFilteringOperations(ContentTableFilterForm filterForm,
            ContentType contentType) {
            if (!(filterForm is PublishingFieldTableFilterForm)) return null;
            var f = filterForm.DirectCastTo<PublishingFieldTableFilterForm>();
            return new[] {
                new ContentTableFilterOperation(PublishingFieldWhereConditionsProvider.IS_PUBLISHED_WHERE_CONDITION_NAME, f)
            };
        }

        public Expression<Func<ProtoContent, bool>> HandleWhere(ContentWhereCondition condition, object param,
            ContentType contentType, out bool callNextHandler) {

            callNextHandler = true;

            if (!condition.IsPublishedCondition() && !(param is PublishingFieldTableFilterForm)) {
                return null;
            }
            var par = param.DirectCastTo<PublishingFieldTableFilterForm>();

            var pfn = $"{ContentType.FIELD_NAME_PUBLISH_STATUS}.{nameof(PublishingField.PublishedUtc)}";
            var ufn = $"{ContentType.FIELD_NAME_PUBLISH_STATUS}.{nameof(PublishingField.UnpublishedUtc)}";

            var now = DateTime.UtcNow;

            if (par.IsPublished) {
                return x => (
                    from pf in x.ContentFields.Where(cf => cf.FieldName == pfn)
                    from uf in x.ContentFields.Where(cf => cf.FieldName == ufn)
                    select new {
                        PublishedUtc = pf.DateTimeValue,
                        UnpublishedUtc = uf.DateTimeValue
                    }).Any(y => y.PublishedUtc != null && y.PublishedUtc.Value <= now &&
                                (y.UnpublishedUtc == null || y.UnpublishedUtc.Value > now)
                );
            }
            return x => !(
                from pf in x.ContentFields.Where(cf => cf.FieldName == pfn)
                from uf in x.ContentFields.Where(cf => cf.FieldName == ufn)
                select new {
                    PublishedUtc = pf.DateTimeValue,
                    UnpublishedUtc = uf.DateTimeValue
                }).Any(y => y.PublishedUtc != null && y.PublishedUtc.Value <= now &&
                            (y.UnpublishedUtc == null || y.UnpublishedUtc.Value > now)
            );
        }
    }
}