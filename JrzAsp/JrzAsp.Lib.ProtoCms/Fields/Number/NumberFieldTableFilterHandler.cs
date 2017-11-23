using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.Number {
    public class NumberFieldTableFilterHandler : IContentTableFilterHandler, IContentWhereHandler {
        public const string NUMBER_FILTER_ID = "number-field-filter";
        public string[] HandledContentTypeIds => new[] {ContentType.ANY_CONTENT_TYPE_ID};
        public decimal Priority => 0;

        public string Id => NUMBER_FILTER_ID;
        public string Name => "Number Field";
        public string Description => "Filter content according to its number fields.";

        public bool CanFilter(ContentType contentType) {
            return true;
        }

        public ContentTableFilterForm BuildFilterForm(ContentType contentType) {
            var fields = contentType.Fields.Where(x => x.ModelType == typeof(NumberField)).ToArray();
            if (fields.Length == 0) return null;
            var fm = new NumberFieldTableFilterForm();
            var items = new List<NumberFieldTableFilterFormItem>();
            foreach (var fi in fields) {
                var fcfg = fi.Config.DirectCastTo<NumberFieldConfiguration>();
                var item = new NumberFieldTableFilterFormItem {
                    FieldName = fi.FieldName,
                    IsNullValue = false,
                    MinNumber = null,
                    GreaterThanOrEqualToMin = false,
                    MaxNumber = null,
                    LessThanOrEqualToMax = false,
                    IsEnabled = false,
                    VueProps = new {
                        label = fcfg.Label ?? fi.FieldName,
                        numberKind = fcfg.NumberKind.ToString().ToLowerInvariant(),
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
                    Name = "cms-widget-number-field-table-filter"
                }
            };
        }

        public ContentTableFilterOperation[] SetupFilteringOperations(ContentTableFilterForm filterForm,
            ContentType contentType) {
            if (!(filterForm is NumberFieldTableFilterForm)) return null;
            var fm = filterForm.DirectCastTo<NumberFieldTableFilterForm>();
            var ops = new List<ContentTableFilterOperation>();
            foreach (var item in fm.Items) {
                ops.Add(new ContentTableFilterOperation(NumberFieldWhereConditionsProvider.CONDITION_NAME, item));
            }
            return ops.ToArray();
        }

        public Expression<Func<ProtoContent, bool>> HandleWhere(ContentWhereCondition condition, object param,
            ContentType contentType,
            out bool callNextHandler) {
            callNextHandler = true;
            if (!(param is NumberFieldTableFilterFormItem)) return null;
            var par = param.DirectCastTo<NumberFieldTableFilterFormItem>();
            if (!par.IsEnabled) return null;
            var fn = $"{par.FieldName}.{nameof(NumberField.Val)}";
            if (par.IsNullValue) {
                return x => x.ContentFields.Any(cf => cf.FieldName == fn && cf.NumberValue == null);
            }
            var min = par.MinNumber ?? decimal.MinValue;
            var max = par.MaxNumber ?? decimal.MaxValue;
            if (par.GreaterThanOrEqualToMin) {
                if (par.LessThanOrEqualToMax) {
                    return x => x.ContentFields.Any(cf => cf.FieldName == fn
                                                          && cf.NumberValue >= min
                                                          && cf.NumberValue <= max);
                }
                return x => x.ContentFields.Any(cf => cf.FieldName == fn
                                                      && cf.NumberValue >= min
                                                      && cf.NumberValue < max);
            }
            if (par.LessThanOrEqualToMax) {
                return x => x.ContentFields.Any(cf => cf.FieldName == fn
                                                      && cf.NumberValue > min
                                                      && cf.NumberValue <= max);
            }
            return x => x.ContentFields.Any(cf => cf.FieldName == fn
                                                  && cf.NumberValue > min
                                                  && cf.NumberValue < max);
        }
    }
}