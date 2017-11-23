using System;
using System.Linq;
using System.Linq.Expressions;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.QueryableUtilities;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.Boolean {
    public class BooleanFieldFinder : IContentFieldFinder {
        public ContentFieldColumn[] Columns => new[] {
            new ContentFieldColumn(
                nameof(BooleanField.Val),
                definition => definition.Config.Label ?? definition.FieldName,
                definition => true,
                GetColumnCellValue,
                GetColumnSummarizedValue,
                GetColumnFullPreviewValue
            )
        };

        private string GetColumnSummarizedValue(ContentField field, ContentFieldDefinition definition) {
            var f = field.DirectCastTo<BooleanField>();
            var label = definition.Config.Label ?? definition.FieldName;
            if (!f.Val.HasValue) {
                return $"{label} unknown";
            }
            if (f.Val.Value) {
                return $"{label}";
            }
            return "";
        }

        private VueComponentDefinition[] GetColumnFullPreviewValue(ContentField field, ContentFieldDefinition definition) {
            return GetColumnCellValue(field, definition);
        }

        private VueComponentDefinition[] GetColumnCellValue(ContentField field, ContentFieldDefinition definition) {
            var f = field.DirectCastTo<BooleanField>();
            var fcfg = definition.Config.DirectCastTo<BooleanFieldConfiguration>();
            if (!f.Val.HasValue) {
                return new VueComponentDefinition[] {
                    new VueHtmlWidget($"<i class='fa fa-question-circle'></i> {fcfg.NullBoolLabel ?? "Unknown"}")
                };
            }
            if (f.Val.Value) {
                return new VueComponentDefinition[] {
                    new VueHtmlWidget($"<i class='fa fa-check-square'></i> {fcfg.TrueBoolLabel ?? "Yes"}")
                };
            }
            return new VueComponentDefinition[] {
                new VueHtmlWidget($"<i class='fa fa-square-o'></i> {fcfg.FalseBoolLabel ?? "Yes"}")
            };
        }

        public ContentField GetModel(ProtoContent content, ContentFieldDefinition fieldDefinition) {
            var mdl = new BooleanField();
            var fn = $"{fieldDefinition.FieldName}.{nameof(BooleanField.Val)}";
            var f = content.ContentFields.FirstOrDefault(x => x.FieldName == fn);
            mdl.Val = f?.BooleanValue;
            return mdl;
        }

        public Expression<Func<ProtoContent, bool>> Search(string keywords, string[] splittedKeywords,
            ContentFieldDefinition fieldDefinition) {
            return null;
        }

        public IQueryable<ProtoContent> Sort(IQueryable<ProtoContent> currentQuery, string sortFieldName,
            bool isDescending,
            ContentFieldDefinition fieldDefinition) {
            var fn = $"{fieldDefinition.FieldName}.{nameof(BooleanField.Val)}";
            if (sortFieldName != fn) return null;
            var sortQ = currentQuery.AddOrderBy(
                x => x.ContentFields.FirstOrDefault(cf => cf.FieldName == fn).BooleanValue, isDescending);
            return sortQ;
        }
    }
}