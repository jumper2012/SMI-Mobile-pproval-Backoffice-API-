using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;

namespace JrzAsp.Lib.ProtoCms.Fields.Common {
    public class CommonFieldModifyOperationsProvider : IContentModifyOperationsProvider {
        public const string CREATE_OPERATION_NAME = "create";
        public const string UPDATE_OPERATION_NAME = "update";
        public const string DELETE_OPERATION_NAME = "delete";

        public decimal Priority => 0;

        public IEnumerable<ContentModifyOperation> DefineModifyOperations() {
            yield return new ContentModifyOperation(
                CREATE_OPERATION_NAME,
                "Create content.",
                ct => $"Create {ct.Name} in CMS",
                ct => $"Enables to create a new {ct.Name} content in CMS.",
                true
            );
            yield return new ContentModifyOperation(
                UPDATE_OPERATION_NAME,
                "Update content.",
                ct => $"Update {ct.Name} in CMS",
                ct => $"Enables to update a {ct.Name} content in CMS.",
                false
            );
            yield return new ContentModifyOperation(
                DELETE_OPERATION_NAME,
                "Delete content.",
                ct => $"Delete {ct.Name} in CMS",
                ct => $"Enables to delete a {ct.Name} content in CMS.",
                false
            );
        }
    }
}