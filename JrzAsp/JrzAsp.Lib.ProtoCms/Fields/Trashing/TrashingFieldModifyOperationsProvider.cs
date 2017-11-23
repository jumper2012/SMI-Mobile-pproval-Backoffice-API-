using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;

namespace JrzAsp.Lib.ProtoCms.Fields.Trashing {
    public class TrashingFieldModifyOperationsProvider : IContentModifyOperationsProvider {
        public const string CHANGE_TRASH_STATUS_OPERATION_NAME = "change-trash-status";

        public decimal Priority => 0;

        public IEnumerable<ContentModifyOperation> DefineModifyOperations() {
            yield return new ContentModifyOperation(
                CHANGE_TRASH_STATUS_OPERATION_NAME,
                "Trash or untrash content.",
                ct => $"Trash {ct.Name} in CMS",
                ct => $"Enables to allow trash/untrash of a {ct.Name} content in CMS.",
                false
            );
        }
    }
}