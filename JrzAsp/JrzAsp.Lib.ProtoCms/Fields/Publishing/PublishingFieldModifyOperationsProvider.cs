using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;

namespace JrzAsp.Lib.ProtoCms.Fields.Publishing {
    public class PublishingFieldModifyOperationsProvider : IContentModifyOperationsProvider {
        public const string CHANGE_PUBLISH_STATUS_OPERATION_NAME = "change-publish-status";

        public decimal Priority => 0;

        public IEnumerable<ContentModifyOperation> DefineModifyOperations() {
            yield return new ContentModifyOperation(
                CHANGE_PUBLISH_STATUS_OPERATION_NAME,
                "Publish or unpublish content.",
                ct => $"Publish {ct.Name} in CMS",
                ct => $"Enables to allow publish/unpublish of a {ct.Name} content in CMS.",
                false
            );
        }
    }
}