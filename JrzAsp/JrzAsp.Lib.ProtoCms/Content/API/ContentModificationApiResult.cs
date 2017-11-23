using JrzAsp.Lib.RepoPattern;
using JrzAsp.Lib.ProtoCms.Core.Models;

namespace JrzAsp.Lib.ProtoCms.Content.API {
    public class ContentModificationApiResult : IModificationApiResult {
        private FurtherValidationResult _validationResult;
        public FurtherValidationResult ValidationResult {
            get {
                if (_validationResult == null) _validationResult = new FurtherValidationResult();
                return _validationResult;
            }
            set => _validationResult = value;
        }
        public bool IsSuccess { get; set; }
        public string ContentId { get; set; }
        public string ContentTypeId { get; set; }
        public string OperationName { get; set; }
    }
}