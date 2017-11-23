using System.Collections.Generic;
using System.Linq;

namespace JrzAsp.Lib.ProtoCms.Fields.Select {
    public class SelectFieldOptionsManager : ISelectFieldOptionsManager {
        private readonly IDictionary<string, ISelectFieldOptionsHandler> _handlerMap =
            new Dictionary<string, ISelectFieldOptionsHandler>();

        public SelectFieldOptionsManager(IEnumerable<ISelectFieldOptionsHandler> handlers) {
            Handlers = handlers.OrderBy(x => x.Priority).ThenBy(x => x.Id).ToArray();
        }

        public ISelectFieldOptionsHandler[] Handlers { get; }

        public ISelectFieldOptionsHandler GetHandler(string id) {
            if (!_handlerMap.TryGetValue(id, out var handler)) {
                handler = Handlers.FirstOrDefault(x => x.Id == id);
                if (handler == null) return null;
                _handlerMap[id] = handler;
            }
            return handler;
        }
    }
}