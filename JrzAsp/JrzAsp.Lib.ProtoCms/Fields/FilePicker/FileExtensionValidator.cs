using System;
using System.Collections.Generic;
using System.Linq;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.RepoPattern;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.FilePicker {
    public class FileExtensionValidator : ContentFieldValidator {
        public override string Name => "file-extension";
        public override string Description => "Validate file extension of file picker field.";
        public override Type ConfigType => typeof(FileExtensionValidatorConfiguration);

        protected override bool CanValidate(Type formType) {
            return typeof(FilePickerFieldModifierForm).IsAssignableFrom(formType);
        }

        protected override FurtherValidationResult Validate(ContentModifierForm form,
            ContentFieldValidatorConfiguration config,
            ContentType contentType, ContentFieldDefinition fieldDefinition) {
            var fm = form.DirectCastTo<FilePickerFieldModifierForm>();
            var cfg = config.DirectCastTo<FileExtensionValidatorConfiguration>();
            if (cfg.AllowedFileExtensions.Length == 0) return null;
            var errs = new List<string>();
            var allowedFileExts = cfg.AllowedFileExtensions.Where(x => x?.Trim().Trim('.').ToLowerInvariant() != null).ToArray();
            var isOrAre = allowedFileExts.Length > 0 ? "are" : "is";
            var allowedFileExtsStr = $"'{string.Join("', '", allowedFileExts)}'";
            foreach (var path in fm.Val) {
                var lastDotIdx = path.LastIndexOf('.');
                var fileExt = path.Substring(lastDotIdx).Trim('.');
                if (allowedFileExts.All(x => x != fileExt.ToLowerInvariant())) {
                    errs.Add($"File extension '{fileExt}' is not allowed. Only {allowedFileExtsStr} {isOrAre} allowed.");
                }
            }
            if (errs.Count == 0) return null;
            var valres = new FurtherValidationResult();
            valres.Errors[nameof(FilePickerFieldModifierForm.Val)] = errs.ToArray();
            return valres;
        }
    }
}