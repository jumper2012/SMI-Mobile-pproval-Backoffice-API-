using System;
using System.Linq;
using System.Linq.Expressions;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.QueryableUtilities;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.Number {
    public class NumberFieldFinder : IContentFieldFinder {
        public ContentFieldColumn[] Columns => new[] {
            new ContentFieldColumn(
                nameof(NumberField.Val),
                definition => definition.Config.Label ?? definition.FieldName,
                definition => true,
                (field, definition) => {
                    var f = field.DirectCastTo<NumberField>();
                    var cfg = definition.Config.DirectCastTo<NumberFieldConfiguration>();
                    var decVal = f.Val;
                    var intVal = f.Val.HasValue ? Convert.ToInt32(f.Val.Value) : null as int?;
                    var isInt = cfg.NumberKind == NumberValueKind.Integer;
                    return new VueComponentDefinition[] {
                        new VueHtmlWidget((isInt ? intVal : decVal)?.ToString() ?? "-n/a-")
                    };
                },
                (field, definition) => {
                    var f = field.DirectCastTo<NumberField>();
                    var cfg = definition.Config.DirectCastTo<NumberFieldConfiguration>();
                    var decVal = f.Val;
                    var intVal = f.Val.HasValue ? Convert.ToInt32(f.Val.Value) : null as int?;
                    var isInt = cfg.NumberKind == NumberValueKind.Integer;
                    return (isInt ? intVal : decVal)?.ToString() ?? "-n/a-";
                },
                (field, definition) => {
                    var f = field.DirectCastTo<NumberField>();
                    var cfg = definition.Config.DirectCastTo<NumberFieldConfiguration>();
                    var decVal = f.Val;
                    var intVal = f.Val.HasValue ? Convert.ToInt32(f.Val.Value) : null as int?;
                    var isInt = cfg.NumberKind == NumberValueKind.Integer;
                    return new VueComponentDefinition[] {
                        new VueHtmlWidget((isInt ? intVal : decVal)?.ToString() ?? "-n/a-")
                    };
                }
            )
        };

        public ContentField GetModel(ProtoContent content, ContentFieldDefinition fieldDefinition) {
            var mdl = new NumberField();
            var fn = $"{fieldDefinition.FieldName}.{nameof(NumberField.Val)}";
            var cf = content.ContentFields.FirstOrDefault(x => x.FieldName == fn);
            mdl.Val = cf?.NumberValue;
            var cfg = fieldDefinition.Config.DirectCastTo<NumberFieldConfiguration>();
            if (mdl.Val.HasValue && cfg.NumberKind == NumberValueKind.Integer) {
                mdl.Val = Convert.ToInt32(mdl.Val.Value);
            }
            return mdl;
        }

        public Expression<Func<ProtoContent, bool>> Search(string keywords, string[] splittedKeywords,
            ContentFieldDefinition fieldDefinition) {
            var cfg = fieldDefinition.Config.DirectCastTo<NumberFieldConfiguration>();
            var fn = $"{fieldDefinition.FieldName}.{nameof(NumberField.Val)}";
            var pred = PredicateBuilder.False<ProtoField>();
            foreach (var kw in splittedKeywords) {
                if (int.TryParse(kw, out var intNum)) {
                    pred = pred.Or(x => x.FieldName == fn && x.NumberValue == intNum);
                }
                if (cfg.NumberKind == NumberValueKind.Decimal && decimal.TryParse(kw, out var decNum)) {
                    var minDec = decNum - cfg.SearchDecimalSensitivity * decNum;
                    var maxDec = decNum + cfg.SearchDecimalSensitivity * decNum;
                    pred = pred.Or(x => x.FieldName == fn && x.NumberValue >= minDec && x.NumberValue <= maxDec);

                }
            }
            Expression<Func<ProtoContent, bool>> cond = x => x.ContentFields.AsQueryable().Any(pred);
            return cond;
        }

        public IQueryable<ProtoContent> Sort(IQueryable<ProtoContent> currentQuery, string sortFieldName,
            bool isDescending,
            ContentFieldDefinition fieldDefinition) {
            var fn = $"{fieldDefinition.FieldName}.{nameof(NumberField.Val)}";
            if (sortFieldName != fn) return null;
            var sortQ = currentQuery.AddOrderBy(
                x => x.ContentFields.FirstOrDefault(cf => cf.FieldName == fn).NumberValue, isDescending);
            return sortQ;
        }
    }
}