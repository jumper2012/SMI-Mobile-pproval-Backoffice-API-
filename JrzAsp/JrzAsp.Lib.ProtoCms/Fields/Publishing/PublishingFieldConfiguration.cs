using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Fields.Common;

namespace JrzAsp.Lib.ProtoCms.Fields.Publishing {
    public class PublishingFieldConfiguration : ContentFieldConfiguration {
        private bool _checkModOpNamesAgain = true;

        private string _dateTimeFormat;

        public PublishingFieldConfiguration() {
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
                    if (!mons.Contains(CommonFieldModifyOperationsProvider.CREATE_OPERATION_NAME)) {
                        mons.Add(CommonFieldModifyOperationsProvider.CREATE_OPERATION_NAME);
                        changed = true;
                    }
                    if (!mons.Contains(CommonFieldModifyOperationsProvider.UPDATE_OPERATION_NAME)) {
                        mons.Add(CommonFieldModifyOperationsProvider.UPDATE_OPERATION_NAME);
                        changed = true;
                    }
                    if (!mons.Contains(PublishingFieldModifyOperationsProvider.CHANGE_PUBLISH_STATUS_OPERATION_NAME)) {
                        mons.Add(PublishingFieldModifyOperationsProvider.CHANGE_PUBLISH_STATUS_OPERATION_NAME);
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