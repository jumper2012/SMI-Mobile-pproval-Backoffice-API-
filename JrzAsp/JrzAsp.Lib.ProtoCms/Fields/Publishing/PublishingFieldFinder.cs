using System;
using System.Linq;
using System.Linq.Expressions;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Fields.Chrono;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.QueryableUtilities;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.Publishing {
    public class PublishingFieldFinder : IContentFieldFinder {
        public ContentFieldColumn[] Columns => new[] {
            new ContentFieldColumn(nameof(PublishingField.PublishedUtc),
                def => "Published UTC",
                def => true,
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<PublishingField>().PublishedUtc
                                          ?.ToString(def.Config.DirectCastTo<PublishingFieldConfiguration>().DateTimeFormat) ?? "-n/a-")
                },
                (field, def) => field.DirectCastTo<PublishingField>().PublishedUtc
                    ?.ToString(def.Config.DirectCastTo<PublishingFieldConfiguration>().DateTimeFormat),
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<PublishingField>().PublishedUtc
                                          ?.ToString(def.Config.DirectCastTo<PublishingFieldConfiguration>().DateTimeFormat) ?? "-n/a-")
                }),
            new ContentFieldColumn(nameof(PublishingField.PublishedAt),
                def => "Published At",
                def => true,
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<PublishingField>().PublishedAt
                                          ?.ToString(def.Config.DirectCastTo<PublishingFieldConfiguration>().DateTimeFormat) ?? "-n/a-")
                },
                (field, def) => field.DirectCastTo<PublishingField>().PublishedAt
                    ?.ToString(def.Config.DirectCastTo<PublishingFieldConfiguration>().DateTimeFormat),
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<PublishingField>().PublishedAt
                                          ?.ToString(def.Config.DirectCastTo<PublishingFieldConfiguration>().DateTimeFormat) ?? "-n/a-")
                }),
            new ContentFieldColumn(nameof(PublishingField.UnpublishedUtc),
                def => "Unpublished UTC",
                def => true,
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<PublishingField>().UnpublishedUtc
                                          ?.ToString(def.Config.DirectCastTo<PublishingFieldConfiguration>().DateTimeFormat) ?? "-n/a-")
                },
                (field, def) => field.DirectCastTo<PublishingField>().UnpublishedUtc
                    ?.ToString(def.Config.DirectCastTo<PublishingFieldConfiguration>().DateTimeFormat),
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<PublishingField>().UnpublishedUtc
                                          ?.ToString(def.Config.DirectCastTo<PublishingFieldConfiguration>().DateTimeFormat) ?? "-n/a-")
                }),
            new ContentFieldColumn(nameof(PublishingField.UnpublishedAt),
                def => "Unpublished At",
                def => true,
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<PublishingField>().UnpublishedAt
                                          ?.ToString(def.Config.DirectCastTo<PublishingFieldConfiguration>().DateTimeFormat) ?? "-n/a-")
                },
                (field, def) => field.DirectCastTo<PublishingField>().UnpublishedAt
                    ?.ToString(def.Config.DirectCastTo<PublishingFieldConfiguration>().DateTimeFormat),
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<PublishingField>().UnpublishedAt
                                          ?.ToString(def.Config.DirectCastTo<PublishingFieldConfiguration>().DateTimeFormat) ?? "-n/a-")
                }),
            new ContentFieldColumn(nameof(PublishingField.IsPublished),
                def => "Is Published?",
                def => true,
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<PublishingField>().IsPublished
                        ? "<i class='fa fa-check-square'></i> Yes"
                        : "<i class='fa fa-square-o'></i> No")
                },
                (field, def) => field.DirectCastTo<PublishingField>().IsPublished ? "Published" : "",
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<PublishingField>().IsPublished
                        ? "<i class='fa fa-check-square'></i> Yes"
                        : "<i class='fa fa-square-o'></i> No")
                }),
            new ContentFieldColumn(nameof(PublishingField.IsDraft),
                def => "Is Draft?",
                def => true,
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<PublishingField>().IsDraft
                        ? "<i class='fa fa-check-square'></i> Yes"
                        : "<i class='fa fa-square-o'></i> No")
                },
                (field, def) => field.DirectCastTo<PublishingField>().IsDraft ? "Draft" : "",
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<PublishingField>().IsDraft
                        ? "<i class='fa fa-check-square'></i> Yes"
                        : "<i class='fa fa-square-o'></i> No")
                })
        };

        public ContentField GetModel(ProtoContent content, ContentFieldDefinition fieldDefinition) {
            var mdl = new PublishingField();
            var pfn = $"{fieldDefinition.FieldName}.{nameof(PublishingField.PublishedUtc)}";
            mdl.PublishedUtc = content.ContentFields.FirstOrDefault(x => x.FieldName == pfn)?.DateTimeValue;
            var ufn = $"{fieldDefinition.FieldName}.{nameof(PublishingField.UnpublishedUtc)}";
            mdl.UnpublishedUtc = content.ContentFields.FirstOrDefault(x => x.FieldName == ufn)?.DateTimeValue;
            return mdl;
        }

        public Expression<Func<ProtoContent, bool>> Search(string keywords, string[] splittedKeywords,
            ContentFieldDefinition fieldDefinition) {
            var cond = PredicateBuilder.False<ProtoContent>();
            var pfn = $"{fieldDefinition.FieldName}.{nameof(PublishingField.PublishedUtc)}";
            var ufn = $"{fieldDefinition.FieldName}.{nameof(PublishingField.UnpublishedUtc)}";
            foreach (var kw in splittedKeywords) {
                if (ChronoUtils.Self.TryBuildSearchConditionExpression<ProtoField>(out var scond, kw,
                    x => x.DateTimeValue, true)) {
                    var pred1 = PredicateBuilder.Create(scond).And(x => x.FieldName == pfn);
                    var pred2 = PredicateBuilder.Create(scond).And(x => x.FieldName == ufn);
                    cond = cond.Or(x => x.ContentFields.AsQueryable().Any(pred1));
                    cond = cond.Or(x => x.ContentFields.AsQueryable().Any(pred2));
                }
            }
            return cond;
        }

        public IQueryable<ProtoContent> Sort(IQueryable<ProtoContent> currentQuery, string sortFieldName,
            bool isDescending,
            ContentFieldDefinition fieldDefinition) {
            var q = currentQuery;
            var pfn = $"{fieldDefinition.FieldName}.{nameof(PublishingField.PublishedUtc)}";
            var ufn = $"{fieldDefinition.FieldName}.{nameof(PublishingField.UnpublishedUtc)}";
            var ipfn = $"{fieldDefinition.FieldName}.{nameof(PublishingField.IsPublished)}";
            var idfn = $"{fieldDefinition.FieldName}.{nameof(PublishingField.IsDraft)}";
            if (sortFieldName == pfn) {
                q = q.AddOrderBy(c => c.ContentFields.FirstOrDefault(x => x.FieldName == pfn).DateTimeValue,
                    isDescending);
            } else if (sortFieldName == ufn) {
                q = q.AddOrderBy(c => c.ContentFields.FirstOrDefault(x => x.FieldName == ufn).DateTimeValue,
                    isDescending);
            } else if (sortFieldName == ipfn) {
                var now = DateTime.UtcNow;
                q = q.AddOrderBy(c =>
                        c.ContentFields.Any(x =>
                            x.FieldName == pfn && x.DateTimeValue.HasValue && x.DateTimeValue.Value <= now) &&
                        c.ContentFields.All(x =>
                            x.FieldName == ufn && (!x.DateTimeValue.HasValue || x.DateTimeValue.Value > now)),
                    isDescending);
            } else if (sortFieldName == idfn) {
                var now = DateTime.UtcNow;
                q = q.AddOrderBy(c =>
                        !c.ContentFields.Any(x =>
                            x.FieldName == pfn && x.DateTimeValue.HasValue && x.DateTimeValue.Value <= now) &&
                        !c.ContentFields.All(x =>
                            x.FieldName == ufn && (!x.DateTimeValue.HasValue || x.DateTimeValue.Value > now)),
                    isDescending);
            }
            return q;
        }
    }
}