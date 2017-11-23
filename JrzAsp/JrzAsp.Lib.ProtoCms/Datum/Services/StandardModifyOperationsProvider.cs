using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Datum.Models;

namespace JrzAsp.Lib.ProtoCms.Datum.Services {
    public class StandardModifyOperationsProvider : IDatumModifyOperationsProvider {
        public const string CREATE_OPERATION_NAME = "create";
        public const string UPDATE_OPERATION_NAME = "update";
        public const string DELETE_OPERATION_NAME = "delete";

        public decimal Priority => 0;

        public IEnumerable<DatumModifyOperation> DefineModifyOperations() {
            yield return new DatumModifyOperation(CREATE_OPERATION_NAME, "Create data.", true);
            yield return new DatumModifyOperation(UPDATE_OPERATION_NAME, "Update data.", false);
            yield return new DatumModifyOperation(DELETE_OPERATION_NAME, "Delete data.", false);
        }
    }
}