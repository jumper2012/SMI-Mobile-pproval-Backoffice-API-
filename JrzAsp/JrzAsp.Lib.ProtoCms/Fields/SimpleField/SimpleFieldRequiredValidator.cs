using System;
using System.Collections;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.RepoPattern;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.SimpleField {
    public class SimpleFieldRequiredValidator : ContentFieldValidator {

        public override string Name => "required";

        public override string Description =>
            "Make sure that the field is not null and if it's a string also not just whitespaces.";

        public override Type ConfigType => typeof(SimpleFieldRequiredValidatorConfiguration);

        protected override bool CanValidate(Type type) {
            var t = FindGenericSimpleFormType(type);
            return t != null;
        }

        protected override FurtherValidationResult Validate(ContentModifierForm form,
            ContentFieldValidatorConfiguration config, ContentType type, ContentFieldDefinition definition) {
            var genericSimpleFormType = FindGenericSimpleFormType(form.GetType());
            var valType = genericSimpleFormType.GenericTypeArguments[0];
            if (valType.IsValueType &&
                !(valType.IsGenericType && valType.GetGenericTypeDefinition() == typeof(Nullable<>))) {
                return null;
            }
            var simpleFieldFormType = typeof(SimpleContentFieldModifierForm<>).MakeGenericType(valType);
            var valProp = simpleFieldFormType.GetProperty(nameof(SimpleContentFieldModifierForm<string>.Val));
            var val = valProp?.GetValue(form);
            var fcfg = definition.Config;
            var cfg = config.DirectCastTo<SimpleFieldRequiredValidatorConfiguration>();
            var dispName = fcfg.Label ?? definition.FieldName;
            var errMsg = cfg.RequiredNullableErrorMessage;
            if (val is string) {
                var valStr = val.DirectCastTo<string>();
                if (!string.IsNullOrWhiteSpace(valStr)) return null;
                errMsg = cfg.RequiredStringErrorMessage;
            } else if (val != null) {
                if (typeof(IEnumerable).IsAssignableFrom(valType)) {
                    //https://stackoverflow.com/questions/5604168/calculating-count-for-ienumerable-non-generic
                    var source = (IEnumerable) val;
                    var count = 0;
                    if (source is ICollection sourceAsCollection) {
                        count = sourceAsCollection.Count;
                    } else {
                        var enumerator = source.GetEnumerator();
                        try {
                            while (enumerator.MoveNext()) {
                                count++;
                            }
                        } finally {
                            if (enumerator is IDisposable disposableObj) {
                                disposableObj.Dispose();
                            }
                        }
                    }
                    if (count > 0) {
                        return null;
                    }
                } else {
                    return null;   
                }
            }
            var vr = new FurtherValidationResult();
            vr.AddError(nameof(SimpleContentFieldModifierForm<string>.Val),
                string.Format(errMsg, new object[] {dispName}));
            return vr;
        }

        private Type FindGenericSimpleFormType(Type type) {
            var t = type;
            var isOk = false;
            while (!isOk && t != null) {
                isOk = t.IsGenericType && t.GetGenericTypeDefinition() == typeof(SimpleContentFieldModifierForm<>);
                if (!isOk) t = t.BaseType;
            }
            return t;
        }
    }
}