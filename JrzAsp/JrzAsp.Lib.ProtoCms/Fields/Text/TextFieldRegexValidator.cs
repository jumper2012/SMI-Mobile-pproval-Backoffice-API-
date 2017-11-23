using System;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.RepoPattern;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.Text {
    public class TextFieldRegexValidator : ContentFieldValidator {
        public override string Name => "regex";
        public override string Description => "Make sure that the text is a valid regex.";
        public override Type ConfigType => typeof(TextFieldRegexValidatorConfiguration);

        protected override bool CanValidate(Type formType) {
            return typeof(TextFieldModifierForm).IsAssignableFrom(formType);
        }

        protected override FurtherValidationResult Validate(ContentModifierForm form,
            ContentFieldValidatorConfiguration config,
            ContentType contentType, ContentFieldDefinition fieldDefinition) {
            var cfg = config.DirectCastTo<TextFieldRegexValidatorConfiguration>();
            var fm = form.DirectCastTo<TextFieldModifierForm>();
            if (fm.Val == null) return null;
            if (!cfg.Regex.IsMatch(fm.Val)) {
                var dispName = fieldDefinition.Config.Label ?? fieldDefinition.FieldName;
                var res = new FurtherValidationResult();
                res.AddError(nameof(fm.Val),
                    string.Format(cfg.ErrorMessage, dispName, cfg.Pattern));
                return res;
            }
            return null;
        }
    }
}