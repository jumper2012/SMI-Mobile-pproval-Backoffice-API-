using System;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.RepoPattern;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.Text {
    public class TextFieldLengthValidator : ContentFieldValidator {
        public override string Name => "length";
        public override string Description => "Make sure that the text string length is within specification.";
        public override Type ConfigType => typeof(TextFieldLengthValidatorConfiguration);

        protected override bool CanValidate(Type formType) {
            return typeof(TextFieldModifierForm).IsAssignableFrom(formType);
        }

        protected override FurtherValidationResult Validate(ContentModifierForm form,
            ContentFieldValidatorConfiguration config,
            ContentType contentType, ContentFieldDefinition fieldDefinition) {
            var valCfg = config.DirectCastTo<TextFieldLengthValidatorConfiguration>();
            var f = form.DirectCastTo<TextFieldModifierForm>();
            if (f.Val == null) return null;
            var result = new FurtherValidationResult();
            var dispName = fieldDefinition.Config.Label ?? fieldDefinition.FieldName;
            if (valCfg.MinLength.HasValue) {
                var ml = valCfg.MinLength.Value;
                if (f.Val.Length < ml) {
                    result.AddError(nameof(f.Val), string.Format(valCfg.MinLengthErrorMessage, dispName, ml));
                }
            }
            if (valCfg.MaxLength.HasValue) {
                var ml = valCfg.MaxLength.Value;
                if (f.Val.Length > ml) {
                    result.AddError(nameof(f.Val), string.Format(valCfg.MaxLengthErrorMessage, dispName, ml));
                }
            }
            return result;
        }
    }
}