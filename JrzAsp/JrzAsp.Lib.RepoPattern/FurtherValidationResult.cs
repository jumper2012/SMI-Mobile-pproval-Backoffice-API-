using System.Collections.Generic;
using System.Linq;

namespace JrzAsp.Lib.RepoPattern {
    /// <summary>
    ///     This class represents the result of further validation of a form model i.e.
    ///     extra validation after standard MVC model validation
    /// </summary>
    public class FurtherValidationResult {
        /// <summary>
        /// Field name that represents global error that's not attached to a particular field.
        /// It's an empty string
        /// </summary>
        public const string GLOBAL_ERROR_FIELD_NAME = "";

        public static FurtherValidationResult Ok => new FurtherValidationResult();

        public FurtherValidationResult() {
            Errors = new Dictionary<string, string[]>();
        }

        public bool HasError {
            get { return Errors.Values.SelectMany(d => d).Any(d => !string.IsNullOrWhiteSpace(d)); }
        }

        /// <summary>
        ///     A dictionary of key-value that represents FieldName-FieldValidationErrorMessages
        ///     If FieldName is "" then it represents global error message
        /// </summary>
        public IDictionary<string, string[]> Errors { get; }

        public string[] GetError() {
            return GetError(GLOBAL_ERROR_FIELD_NAME);
        }

        public void AddError(string message) {
            AddError(GLOBAL_ERROR_FIELD_NAME, message);
        }

        public string[] GetError(string fieldName) {
            string[] msgs;

            if (Errors.TryGetValue(fieldName, out msgs)) return msgs;

            return new string[0];
        }

        public void AddError(string fieldName, string message) {
            string[] msgs;
            List<string> newMsgs;

            if (Errors.TryGetValue(fieldName, out msgs)) {
                newMsgs = new List<string>(msgs);
            } else {
                newMsgs = new List<string>();
            }

            newMsgs.Add(message);
            Errors[fieldName] = newMsgs.ToArray();
        }
    }
}