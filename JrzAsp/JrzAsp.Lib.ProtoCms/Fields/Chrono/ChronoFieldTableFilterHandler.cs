using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.Chrono {
    public class ChronoFieldTableFilterHandler : IContentTableFilterHandler, IContentWhereHandler {
        public const string CHRONO_FILTER_ID = "chrono-field-filter";
        public string[] HandledContentTypeIds => new[] {ContentType.ANY_CONTENT_TYPE_ID};
        public decimal Priority => 0;

        public string Id => CHRONO_FILTER_ID;
        public string Name => "Date/Time Field";
        public string Description => "Filter content according to its datetime fields.";

        public bool CanFilter(ContentType contentType) {
            return true;
        }

        public ContentTableFilterForm BuildFilterForm(ContentType contentType) {
            var fields = contentType.Fields.Where(x => x.ModelType == typeof(ChronoField)).ToArray();
            if (fields.Length == 0) return null;
            var fm = new ChronoFieldTableFilterForm();
            var items = new List<ChronoFieldTableFilterFormItem>();
            foreach (var fi in fields) {
                var fcfg = fi.Config.DirectCastTo<ChronoFieldConfiguration>();
                var item = new ChronoFieldTableFilterFormItem {
                    FieldName = fi.FieldName,
                    IsNullValue = false,
                    MinDateTime = null,
                    GreaterThanOrEqualToMin = false,
                    MaxDateTime = null,
                    LessThanOrEqualToMax = false,
                    IsEnabled = false,
                    VueProps = new {
                        label = fcfg.Label ?? fi.FieldName,
                        pickerKind = fcfg.PickerKind.ToString(),
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
                    Name = "cms-widget-chrono-field-table-filter"
                }
            };
        }

        public ContentTableFilterOperation[] SetupFilteringOperations(ContentTableFilterForm filterForm,
            ContentType contentType) {
            if (!(filterForm is ChronoFieldTableFilterForm)) return null;
            var fm = filterForm.DirectCastTo<ChronoFieldTableFilterForm>();
            var ops = new List<ContentTableFilterOperation>();
            foreach (var item in fm.Items) {
                ops.Add(new ContentTableFilterOperation(ChronoFieldWhereConditionsProvider.CONDITION_NAME, item));
            }
            return ops.ToArray();
        }

        public Expression<Func<ProtoContent, bool>> HandleWhere(ContentWhereCondition condition, object param,
            ContentType contentType,
            out bool callNextHandler) {
            callNextHandler = true;
            if (!(param is ChronoFieldTableFilterFormItem)) return null;
            var par = param.DirectCastTo<ChronoFieldTableFilterFormItem>();
            if (!par.IsEnabled) return null;
            var fn = $"{par.FieldName}.{nameof(ChronoField.Val)}";
            if (par.IsNullValue) {
                return x => x.ContentFields.Any(cf => cf.FieldName == fn && cf.DateTimeValue == null);
            }
            var min = par.MinDateTime ?? DateTime.Now;
            var max = par.MaxDateTime ?? DateTime.Now;
            if (par.GreaterThanOrEqualToMin) {
                if (par.LessThanOrEqualToMax) {
                    return x => x.ContentFields.Any(cf => cf.FieldName == fn
                                                          && cf.DateTimeValue >= min
                                                          && cf.DateTimeValue <= max);
                }
                return x => x.ContentFields.Any(cf => cf.FieldName == fn
                                                      && cf.DateTimeValue >= min
                                                      && cf.DateTimeValue < max);
            }
            if (par.LessThanOrEqualToMax) {
                return x => x.ContentFields.Any(cf => cf.FieldName == fn
                                                      && cf.DateTimeValue > min
                                                      && cf.DateTimeValue <= max);
            }
            return x => x.ContentFields.Any(cf => cf.FieldName == fn
                                                  && cf.DateTimeValue > min
                                                  && cf.DateTimeValue < max);
        }
    }
}