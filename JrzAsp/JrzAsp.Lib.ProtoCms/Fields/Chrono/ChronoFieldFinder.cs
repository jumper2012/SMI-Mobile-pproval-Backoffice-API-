using System;
using System.Linq;
using System.Linq.Expressions;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.QueryableUtilities;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.Chrono {
    public class ChronoFieldFinder : IContentFieldFinder {

        public ContentFieldColumn[] Columns => new[] {
            new ContentFieldColumn(nameof(ChronoField.Val),
                definition => definition.Config.DirectCastTo<ChronoFieldConfiguration>().Label ?? definition.FieldName,
                definition => true,
                (field, definition) => {
                    var f = field.DirectCastTo<ChronoField>();
                    var fcfg = definition.Config.DirectCastTo<ChronoFieldConfiguration>();
                    var dtVal = fcfg.Kind == DateTimeKind.Utc ? f.ValAsUtcDateTime : f.ValAsLocalDateTime;
                    return new VueComponentDefinition[] {
                        new VueHtmlWidget(dtVal?.ToString(fcfg.DateTimeFormat) ?? "-n/a-")
                    };
                },
                (field, definition) => {
                    var f = field.DirectCastTo<ChronoField>();
                    var fcfg = definition.Config.DirectCastTo<ChronoFieldConfiguration>();
                    var dtVal = fcfg.Kind == DateTimeKind.Utc ? f.ValAsUtcDateTime : f.ValAsLocalDateTime;
                    return dtVal?.ToString(fcfg.DateTimeFormat) ?? "";
                },
                (field, definition) => {
                    var f = field.DirectCastTo<ChronoField>();
                    var fcfg = definition.Config.DirectCastTo<ChronoFieldConfiguration>();
                    var dtVal = fcfg.Kind == DateTimeKind.Utc ? f.ValAsUtcDateTime : f.ValAsLocalDateTime;
                    return new VueComponentDefinition[] {
                        new VueHtmlWidget(dtVal?.ToString(fcfg.DateTimeFormat) ?? "-n/a-")
                    };
                })
        };

        public ContentField GetModel(ProtoContent content, ContentFieldDefinition fieldDefinition) {
            var mdl = new ChronoField();
            var fn = $"{fieldDefinition.FieldName}.{nameof(ChronoField.Val)}";
            mdl.Val = content.ContentFields.FirstOrDefault(x => x.FieldName == fn)?.DateTimeValue;
            var cfg = fieldDefinition.Config.DirectCastTo<ChronoFieldConfiguration>();
            if (mdl.Val.HasValue) {
                mdl.Val = DateTime.SpecifyKind(mdl.Val.Value, cfg.Kind);
            }
            return mdl;
        }

        public Expression<Func<ProtoContent, bool>> Search(string keywords, string[] splittedKeywords,
            ContentFieldDefinition fieldDefinition) {
            var cfg = fieldDefinition.Config.DirectCastTo<ChronoFieldConfiguration>();
            var fn = $"{fieldDefinition.FieldName}.{nameof(ChronoField.Val)}";
            var pred = PredicateBuilder.False<ProtoContent>();
            foreach (var kw in splittedKeywords) {
                if (ChronoUtils.Self.TryBuildSearchConditionExpression<ProtoField>(out var scond, kw,
                    x => x.DateTimeValue, cfg.Kind == DateTimeKind.Utc)) {
                    var cond = PredicateBuilder.Create(scond).And(x => x.FieldName == fn);
                    pred = pred.Or(x => x.ContentFields.AsQueryable().Any(cond));
                }
                return pred;
            }
            return pred;
        }

        public IQueryable<ProtoContent> Sort(IQueryable<ProtoContent> currentQuery, string sortFieldName,
            bool isDescending,
            ContentFieldDefinition fieldDefinition) {
            var q = currentQuery;
            var fn = $"{fieldDefinition.FieldName}.{nameof(ChronoField.Val)}";
            if (sortFieldName == fieldDefinition.FieldName || sortFieldName == fn) {
                q = q.AddOrderBy(c => c.ContentFields.FirstOrDefault(x => x.FieldName == fn).DateTimeValue,
                    isDescending);
            }
            return q;
        }
    }
}