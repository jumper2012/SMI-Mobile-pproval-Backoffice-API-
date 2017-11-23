using System;
using System.Linq;
using System.Linq.Expressions;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Fields.Chrono;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.QueryableUtilities;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.Trashing {
    public class TrashingFieldFinder : IContentFieldFinder {
        public ContentFieldColumn[] Columns => new[] {
            new ContentFieldColumn(nameof(TrashingField.TrashedUtc),
                def => "Trashed UTC",
                def => true,
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<TrashingField>().TrashedUtc
                                          ?.ToString(def.Config.DirectCastTo<TrashingFieldConfiguration>().DateTimeFormat) ?? "-n/a-")
                },
                (field, def) => field.DirectCastTo<TrashingField>().TrashedUtc
                    ?.ToString(def.Config.DirectCastTo<TrashingFieldConfiguration>().DateTimeFormat),
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<TrashingField>().TrashedUtc
                                          ?.ToString(def.Config.DirectCastTo<TrashingFieldConfiguration>().DateTimeFormat) ?? "-n/a-")
                }),
            new ContentFieldColumn(nameof(TrashingField.TrashedAt),
                def => "Trashed At",
                def => true,
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<TrashingField>().TrashedAt
                                          ?.ToString(def.Config.DirectCastTo<TrashingFieldConfiguration>().DateTimeFormat) ?? "-n/a-")
                },
                (field, def) => field.DirectCastTo<TrashingField>().TrashedAt
                    ?.ToString(def.Config.DirectCastTo<TrashingFieldConfiguration>().DateTimeFormat),
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<TrashingField>().TrashedAt
                                          ?.ToString(def.Config.DirectCastTo<TrashingFieldConfiguration>().DateTimeFormat) ?? "-n/a-")
                }),
            new ContentFieldColumn(nameof(TrashingField.IsTrashed),
                def => "Is Trashed?",
                def => true,
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<TrashingField>().IsTrashed
                        ? "<i class='fa fa-check-square'></i> Yes"
                        : "<i class='fa fa-square-o'></i> No")
                },
                (field, def) => field.DirectCastTo<TrashingField>().IsTrashed ? "Trashed" : "",
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<TrashingField>().IsTrashed
                        ? "<i class='fa fa-check-square'></i> Yes"
                        : "<i class='fa fa-square-o'></i> No")
                })
        };

        public ContentField GetModel(ProtoContent content, ContentFieldDefinition fieldDefinition) {
            var mdl = new TrashingField();
            var tfn = $"{fieldDefinition.FieldName}.{nameof(TrashingField.TrashedUtc)}";
            mdl.TrashedUtc = content.ContentFields.FirstOrDefault(x => x.FieldName == tfn)?.DateTimeValue;
            return mdl;
        }

        public Expression<Func<ProtoContent, bool>> Search(string keywords, string[] splittedKeywords,
            ContentFieldDefinition fieldDefinition) {
            var cond = PredicateBuilder.False<ProtoContent>();
            var tfn = $"{fieldDefinition.FieldName}.{nameof(TrashingField.TrashedUtc)}";
            foreach (var kw in splittedKeywords) {
                if (ChronoUtils.Self.TryBuildSearchConditionExpression<ProtoField>(out var scond, kw,
                    x => x.DateTimeValue, true)) {
                    var pred = PredicateBuilder.Create(scond).And(x => x.FieldName == tfn);
                    cond = cond.Or(x => x.ContentFields.AsQueryable().Any(pred));
                }
            }
            return cond;
        }

        public IQueryable<ProtoContent> Sort(IQueryable<ProtoContent> currentQuery, string sortFieldName,
            bool isDescending,
            ContentFieldDefinition fieldDefinition) {
            var q = currentQuery;
            var tfn = $"{fieldDefinition.FieldName}.{nameof(TrashingField.TrashedUtc)}";
            var itfn = $"{fieldDefinition.FieldName}.{nameof(TrashingField.IsTrashed)}";
            if (sortFieldName == tfn) {
                q = q.AddOrderBy(c => c.ContentFields.FirstOrDefault(x => x.FieldName == tfn).DateTimeValue,
                    isDescending);
            } else if (sortFieldName == itfn) {
                var now = DateTime.UtcNow;
                q = q.AddOrderBy(c =>
                    c.ContentFields.Any(x =>
                        x.FieldName == tfn && x.DateTimeValue.HasValue && x.DateTimeValue.Value <= now), isDescending);
            }
            return q;
        }
    }
}