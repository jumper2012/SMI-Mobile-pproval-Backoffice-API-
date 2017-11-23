using JrzAsp.Lib.RepoPattern;

namespace JrzAsp.Lib.ProtoCms.Core.Models {
    public interface IModificationApiResult {
        FurtherValidationResult ValidationResult { get; }
        bool IsSuccess { get; }
    }
}