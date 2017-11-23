using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Fields.Common;

namespace JrzAsp.Lib.ProtoCms.Fields.Trashing {
    public class TrashingFieldConfiguration : ContentFieldConfiguration {
        private bool _checkModOpNamesAgain = true;
        private string _dateTimeFormat;

        public TrashingFieldConfiguration() {
            HandledModifyOperationNames = new string[0];
        }

        public string DateTimeFormat {
            get {
                if (_dateTimeFormat != null) return _dateTimeFormat;
                _dateTimeFormat = ProtoContent.DATE_FORMAT_FOR_DISPLAY;
                return _dateTimeFormat;
            }
            set => _dateTimeFormat = value;
        }

        public override string[] HandledModifyOperationNames {
            get {
                if (_checkModOpNamesAgain) {
                    var changed = false;
                    var mons = new List<string>(base.HandledModifyOperationNames);
                    if (!mons.Contains(CommonFieldModifyOperationsProvider.UPDATE_OPERATION_NAME)) {
                        mons.Add(CommonFieldModifyOperationsProvider.UPDATE_OPERATION_NAME);
                        changed = true;
                    }
                    if (!mons.Contains(TrashingFieldModifyOperationsProvider.CHANGE_TRASH_STATUS_OPERATION_NAME)) {
                        mons.Add(TrashingFieldModifyOperationsProvider.CHANGE_TRASH_STATUS_OPERATION_NAME);
                        changed = true;
                    }
                    if (changed) {
                        base.HandledModifyOperationNames = mons.ToArray();
                    }
                    _checkModOpNamesAgain = false;
                }
                return base.HandledModifyOperationNames;
            }
            set {
                base.HandledModifyOperationNames = value;
                _checkModOpNamesAgain = true;
            }

        }
    }
}