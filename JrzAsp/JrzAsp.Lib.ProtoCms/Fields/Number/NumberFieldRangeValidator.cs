using System;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.RepoPattern;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.Number {
    public class NumberFieldRangeValidator : ContentFieldValidator {
        public override string Name => "range";

        public override string Description =>
            "Validate number field so that it's between min and max value.";

        public override Type ConfigType => typeof(NumberFieldRangeValidatorConfiguration);

        protected override bool CanValidate(Type formType) {
            return typeof(NumberFieldModifierForm).IsAssignableFrom(formType);
        }

        protected override FurtherValidationResult Validate(ContentModifierForm form,
            ContentFieldValidatorConfiguration config,
            ContentType contentType, ContentFieldDefinition fieldDefinition) {
            var f = form.DirectCastTo<NumberFieldModifierForm>();
            var valCfg = config.DirectCastTo<NumberFieldRangeValidatorConfiguration>();
            if (!f.Val.HasValue) return null;
            var result = new FurtherValidationResult();
            var dispName = fieldDefinition.Config.Label ?? fieldDefinition.FieldName;
            if (valCfg.MinValue.HasValue) {
                var mv = valCfg.MinValue.Value;
                if (valCfg.MinValueIsInclusive) {
                    if (f.Val.Value < mv) {
                        result.AddError(
                            nameof(f.Val), string.Format(valCfg.MinValueInclusiveErrorMessage, dispName, mv));
                    }
                } else {
                    if (f.Val.Value <= mv) {
                        result.AddError(
                            nameof(f.Val), string.Format(valCfg.MinValueErrorMessage, dispName, mv));
                    }
                }
            }
            if (valCfg.MaxValue.HasValue) {
                var mv = valCfg.MaxValue.Value;
                if (valCfg.MaxValueIsInclusive) {
                    if (f.Val.Value > mv) {
                        result.AddError(
                            nameof(f.Val), string.Format(valCfg.MaxValueInclusiveErrorMessage, dispName, mv));
                    }
                } else {
                    if (f.Val.Value >= mv) {
                        result.AddError(
                            nameof(f.Val), string.Format(valCfg.MaxValueErrorMessage, dispName, mv));
                    }
                }
            }
            return result;
        }
    }
}