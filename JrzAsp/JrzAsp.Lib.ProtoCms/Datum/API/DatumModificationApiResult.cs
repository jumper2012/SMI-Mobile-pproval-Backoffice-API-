using JrzAsp.Lib.RepoPattern;
using JrzAsp.Lib.ProtoCms.Core.Models;

namespace JrzAsp.Lib.ProtoCms.Datum.API {
    public class DatumModificationApiResult : IModificationApiResult {
        private FurtherValidationResult _validationResult;
        public FurtherValidationResult ValidationResult {
            get {
                if (_validationResult == null) _validationResult = new FurtherValidationResult();
                return _validationResult;
            }
            set => _validationResult = value;
        }
        public bool IsSuccess { get; set; }
        public string DatumId { get; set; }
        public string DatumTypeId { get; set; }
        public string OperationName { get; set; }
    }
}