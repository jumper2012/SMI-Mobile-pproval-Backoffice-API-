using System;
using System.Linq;
using System.Linq.Expressions;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.QueryableUtilities;
using JrzAsp.Lib.TextUtilities;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.Text {
    public class TextFieldFinder : IContentFieldFinder {
        public ContentFieldColumn[] Columns => new[] {
            new ContentFieldColumn(nameof(TextField.Val),
                definition => definition.Config.DirectCastTo<TextFieldConfiguration>().Label ?? definition.FieldName,
                definition => true,
                (field, definition) => {
                    var shortened = GetSummaryValue(field, definition);
                    return new VueComponentDefinition[] {
                        new VueHtmlWidget(shortened)
                    };
                },
                (field, definition) => {
                    var shortened = GetSummaryValue(field, definition);
                    return shortened;
                },
                (field, definition) => {
                    var f = field.DirectCastTo<TextField>();
                    return new VueComponentDefinition[] {
                        new VueHtmlWidget(f.Val)
                    };
                })
        };

        public ContentField GetModel(ProtoContent content, ContentFieldDefinition fieldDefinition) {
            var mdl = new TextField();
            var fn = $"{fieldDefinition.FieldName}.{nameof(TextField.Val)}";
            var cf = content.ContentFields.FirstOrDefault(x => x.FieldName == fn);
            mdl.Val = cf?.StringValue;
            return mdl;
        }

        public Expression<Func<ProtoContent, bool>> Search(string keywords, string[] splittedKeywords,
            ContentFieldDefinition fieldDefinition) {
            var fn = $"{fieldDefinition.FieldName}.{nameof(TextField.Val)}";
            Expression<Func<ProtoContent, bool>> expr = x => x.ContentFields.Any(cf =>
                 cf.FieldName == fn && splittedKeywords.Any(
                     k => cf.StringValue.Contains(k)
                 )
             );
            return expr;
        }

        public IQueryable<ProtoContent> Sort(IQueryable<ProtoContent> currentQuery, string sortFieldName,
            bool isDescending,
            ContentFieldDefinition fieldDefinition) {
            var fn = $"{fieldDefinition.FieldName}.{nameof(TextField.Val)}";
            if (sortFieldName != fn) return null;
            var sortQ = currentQuery.AddOrderBy(x =>
                x.ContentFields.FirstOrDefault(cf => cf.FieldName == fn).StringValue, isDescending);
            return sortQ;
        }

        private string GetSummaryValue(ContentField field, ContentFieldDefinition definition) {
            var f = field.DirectCastTo<TextField>();
            var cfg = definition.Config.DirectCastTo<TextFieldConfiguration>();
            var stripped = f.Val.StripHtmlTags();
            var shortened = stripped == null ? "" : (stripped.Length > cfg.MaxSummaryLength
                ? stripped.Substring(0, cfg.MaxSummaryLength) + "..."
                : stripped);
            return shortened;
        }
    }
}