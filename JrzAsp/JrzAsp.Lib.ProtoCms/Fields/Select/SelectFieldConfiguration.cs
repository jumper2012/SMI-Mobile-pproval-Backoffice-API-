using System;
using JrzAsp.Lib.ProtoCms.Content.Models;

namespace JrzAsp.Lib.ProtoCms.Fields.Select {
    public class SelectFieldConfiguration : ContentFieldConfiguration {
        private static IDependencyProvider _dp = ProtoEngine.GetDependencyProvider();

        private string _optionsHandlerId;
        public string OptionsHandlerId {
            get {
                if (_optionsHandlerId == null) {
                    throw new InvalidOperationException(
                        $"ProtoCMS: {typeof(SelectFieldConfiguration).FullName} requires " +
                        $"{nameof(OptionsHandlerId)} to function.");
                }
                return _optionsHandlerId;
            }
            set { _optionsHandlerId = value; }
        }
        public string OptionsHandlerParam { get; set; }
        public bool IsMultiSelect { get; set; }

        public ISelectFieldOptionsHandler OptionsHandler() {
            var mgr = _dp.GetService<ISelectFieldOptionsManager>();
            return mgr.GetHandler(OptionsHandlerId);
        }
    }
}