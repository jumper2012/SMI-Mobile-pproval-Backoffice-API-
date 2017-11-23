using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Fields.Common;

namespace JrzAsp.Lib.ProtoCms.Content.Models {
    public class ContentFieldConfiguration {

        private string[] _modifyOperationNames;
        private IDictionary<string, ContentFieldValidatorConfiguration> _validators;
        
        /// <summary>
        ///     The label to use for this field. This may not work as expected if for complex field.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Help text to show in form field editor
        /// </summary>
        public string HelpText { get; set; }

        /// <summary>
        ///     Whether to include this field when searching for content. Defaults to true
        /// </summary>
        public bool IncludeWhenSearching { get; set; } = true;

        /// <summary>
        ///     The modify operation names where this field modifier handler should perform.
        ///     <br />
        ///     Default values for this field are '<see cref="CommonFieldModifyOperationsProvider.CREATE_OPERATION_NAME" />',
        ///     and '<see cref="CommonFieldModifyOperationsProvider.UPDATE_OPERATION_NAME" />'
        /// </summary>
        public virtual string[] HandledModifyOperationNames {
            get => _modifyOperationNames ?? (_modifyOperationNames = new[] {
                CommonFieldModifyOperationsProvider.CREATE_OPERATION_NAME,
                CommonFieldModifyOperationsProvider.UPDATE_OPERATION_NAME
            });
            set => _modifyOperationNames = value;
        }

        /// <summary>
        ///     Define validators to use to validate this field. The dictionary key is <b>the validator name</b>,
        ///     and the value is <b>the validator config object</b>.
        /// </summary>
        public IDictionary<string, ContentFieldValidatorConfiguration> Validators {
            get => _validators ?? (_validators = new Dictionary<string, ContentFieldValidatorConfiguration>());
            set => _validators = value;
        }
    }
}