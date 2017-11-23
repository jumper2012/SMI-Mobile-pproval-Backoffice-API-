using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.Boolean {
    public class BooleanFieldTableFilterHandler : IContentTableFilterHandler, IContentWhereHandler {
        public const string BOOLEAN_FILTER_ID = "boolean-field-filter";
        public string[] HandledContentTypeIds => new[] {ContentType.ANY_CONTENT_TYPE_ID};
        public decimal Priority => 0;

        public string Id => BOOLEAN_FILTER_ID;
        public string Name => "Boolean Field";
        public string Description => "Filter content according to its boolean fields.";

        public bool CanFilter(ContentType contentType) {
            return true;
        }

        public ContentTableFilterForm BuildFilterForm(ContentType contentType) {
            var fields = contentType.Fields.Where(x => x.ModelType == typeof(BooleanField)).ToArray();
            if (fields.Length == 0) return null;
            var fm = new BooleanFieldTableFilterForm();
            var items = new List<BooleanFieldTableFilterFormItem>();
            foreach (var fi in fields) {
                var fcfg = fi.Config.DirectCastTo<BooleanFieldConfiguration>();
                var item = new BooleanFieldTableFilterFormItem {
                    FieldName = fi.FieldName,
                    IsChecked = false,
                    IsEnabled = false,
                    VueProps = new {
                        label = fcfg.Label ?? fi.FieldName,
                        yesLabel = fcfg.TrueBoolLabel,
                        noLabel = fcfg.FalseBoolLabel,
                        helpText = fcfg.HelpText
                    }
                };
                items.Add(item);
            }
            fm.Items = items.ToArray();
            return fm;
        }

        public VueComponentDefinition[] FilterFormVues(ContentTableFilterForm filterForm, ContentType contentType) {
            return new[] {
                new VueComponentDefinition {
                    Name = "cms-widget-boolean-field-table-filter"
                }
            };
        }

        public ContentTableFilterOperation[] SetupFilteringOperations(ContentTableFilterForm filterForm,
            ContentType contentType) {
            if (!(filterForm is BooleanFieldTableFilterForm)) return null;
            var fm = filterForm.DirectCastTo<BooleanFieldTableFilterForm>();
            var ops = new List<ContentTableFilterOperation>();
            foreach (var item in fm.Items) {
                ops.Add(new ContentTableFilterOperation(BooleanFieldWhereConditionsProvider.CONDITION_NAME, item));
            }
            return ops.ToArray();
        }

        public Expression<Func<ProtoContent, bool>> HandleWhere(ContentWhereCondition condition, object param,
            ContentType contentType,
            out bool callNextHandler) {
            callNextHandler = true;
            if (!(param is BooleanFieldTableFilterFormItem)) return null;
            var par = param.DirectCastTo<BooleanFieldTableFilterFormItem>();
            if (!par.IsEnabled) return null;
            var fn = $"{par.FieldName}.{nameof(BooleanField.Val)}";
            var isCheck = par.IsChecked;
            // content field boolean value can be null, true, or false, so null is considered false
            if (isCheck) {
                return x => x.ContentFields.Any(cf => cf.FieldName == fn && cf.BooleanValue == true);
            }
            return x => x.ContentFields.Any(cf => cf.FieldName == fn && cf.BooleanValue != true);
        }
    }
}