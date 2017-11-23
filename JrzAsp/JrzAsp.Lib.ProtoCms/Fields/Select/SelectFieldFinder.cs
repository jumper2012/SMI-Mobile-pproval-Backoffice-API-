using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.QueryableUtilities;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.Select {
    public class SelectFieldFinder : IContentFieldFinder {
        public ContentFieldColumn[] Columns => new[] {
            new ContentFieldColumn(
                nameof(SelectField.Val),
                ColumnHeader,
                fdef => true,
                CellValue,
                SummarizedValue,
                FullPreviewValue
            )
        };

        public ContentField GetModel(ProtoContent content, ContentFieldDefinition fieldDefinition) {
            var mdl = new SelectField();
            var fcfg = fieldDefinition.Config.DirectCastTo<SelectFieldConfiguration>();
            var fnPrefix = $"{fieldDefinition.FieldName}.{nameof(SelectField.Val)}.";
            if (!fcfg.IsMultiSelect) {
                var cf = content.ContentFields.FirstOrDefault(x => x.FieldName.StartsWith(fnPrefix));
                if (cf?.StringValue != null) {
                    mdl.Val = new[] {
                        cf.StringValue
                    };
                }
            } else {
                var cfs = content.ContentFields.Where(
                    x => x.FieldName.StartsWith(fnPrefix)).ToArray();
                var vals = new List<Tuple<int, string>>();
                foreach (var cf in cfs) {
                    var idxStr = cf.FieldName.Replace(fnPrefix, "");
                    if (!int.TryParse(idxStr, out var idx)) continue;
                    if (cf.StringValue != null) {
                        vals.Add(Tuple.Create(idx, cf.StringValue));
                    }
                }
                mdl.Val = vals.OrderBy(x => x.Item1).Select(x => x.Item2).ToArray();
            }
            return mdl;
        }

        public Expression<Func<ProtoContent, bool>> Search(string keywords, string[] splittedKeywords,
            ContentFieldDefinition fieldDefinition) {
            var fnPrefix = $"{fieldDefinition.FieldName}.{nameof(SelectField.Val)}.";
            return x => splittedKeywords.Any(k =>
                x.ContentFields.Any(cf => cf.FieldName.StartsWith(fnPrefix) && cf.StringValue.Contains(k)));
        }

        public IQueryable<ProtoContent> Sort(IQueryable<ProtoContent> currentQuery, string sortFieldName,
            bool isDescending,
            ContentFieldDefinition fieldDefinition) {
            var fnPrefix = $"{fieldDefinition.FieldName}.{nameof(SelectField.Val)}.";
            return currentQuery.AddOrderBy(
                x => x.ContentFields.FirstOrDefault(cf => cf.FieldName.StartsWith(fnPrefix)).StringValue,
                isDescending);
        }

        private VueComponentDefinition[] FullPreviewValue(ContentField contentField,
            ContentFieldDefinition contentFieldDefinition) {
            var field = contentField.DirectCastTo<SelectField>();
            var fcfg = contentFieldDefinition.Config.DirectCastTo<SelectFieldConfiguration>();
            var handler = fcfg.OptionsHandler();
            if (!fcfg.IsMultiSelect) {
                var val = field.Val.FirstOrDefault();
                var html = "-n/a-";
                if (val != null) {
                    var opt = handler.GetOptionObject(val, fcfg.OptionsHandlerParam);
                    if (opt != null) {
                        html = $"<li>{handler.GetOptionDisplay(opt, fcfg.OptionsHandlerParam).Label} <code class='small text-muted'>{val}</code></li>";
                    }
                }
                return new VueComponentDefinition[] {
                    new VueHtmlWidget(html)
                };
            }
            var vues = new List<VueComponentDefinition>();
            foreach (var val in field.Val) {
                var html = "-n/a-";
                if (val != null) {
                    var opt = handler.GetOptionObject(val, fcfg.OptionsHandlerParam);
                    if (opt != null) {
                        html = $"<li>{handler.GetOptionDisplay(opt, fcfg.OptionsHandlerParam).Label} <code class='small text-muted'>{val}</code></li>";
                    }
                }
                vues.Add(new VueHtmlWidget(html));
            }
            if (vues.Count == 0) {
                vues.Add(new VueHtmlWidget("-n/a-"));
            }
            return vues.ToArray();
        }

        private string SummarizedValue(ContentField contentField, ContentFieldDefinition contentFieldDefinition) {
            var field = contentField.DirectCastTo<SelectField>();
            var fcfg = contentFieldDefinition.Config.DirectCastTo<SelectFieldConfiguration>();
            var handler = fcfg.OptionsHandler();
            if (!fcfg.IsMultiSelect) {
                var val = field.Val.FirstOrDefault();
                var sumval = "-n/a-";
                if (val != null) {
                    var opt = handler.GetOptionObject(val, fcfg.OptionsHandlerParam);
                    if (opt != null) {
                        sumval = $"{handler.GetOptionDisplay(opt, fcfg.OptionsHandlerParam).Label}";
                    }
                }
                return sumval;
            }
            return $"{field.Val.Length} items in {fcfg.Label} field";
        }

        private VueComponentDefinition[] CellValue(ContentField contentField,
            ContentFieldDefinition contentFieldDefinition) {
            var field = contentField.DirectCastTo<SelectField>();
            var fcfg = contentFieldDefinition.Config.DirectCastTo<SelectFieldConfiguration>();
            var handler = fcfg.OptionsHandler();
            if (!fcfg.IsMultiSelect) {
                var val = field.Val.FirstOrDefault();
                var html = "-n/a-";
                if (val != null) {
                    var opt = handler.GetOptionObject(val, fcfg.OptionsHandlerParam);
                    if (opt != null) {
                        html = $"<li>{handler.GetOptionDisplay(opt, fcfg.OptionsHandlerParam).Label}</li>";
                    }
                }
                return new VueComponentDefinition[] {
                    new VueHtmlWidget(html)
                };
            }
            var vues = new List<VueComponentDefinition>();
            foreach (var val in field.Val) {
                var html = "-n/a-";
                if (val != null) {
                    var opt = handler.GetOptionObject(val, fcfg.OptionsHandlerParam);
                    if (opt != null) {
                        html = $"<li>{handler.GetOptionDisplay(opt, fcfg.OptionsHandlerParam).Label}</li>";
                    }
                }
                vues.Add(new VueHtmlWidget(html));
            }
            if (vues.Count == 0) {
                vues.Add(new VueHtmlWidget("-n/a-"));
            }
            return vues.ToArray();
        }

        private string ColumnHeader(ContentFieldDefinition contentFieldDefinition) {
            var fcfg = contentFieldDefinition.Config.DirectCastTo<SelectFieldConfiguration>();
            return fcfg.Label ?? contentFieldDefinition.FieldName;
        }
    }
}