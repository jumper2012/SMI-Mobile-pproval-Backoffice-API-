using System;
using System.Collections.Concurrent;
using System.Linq;

namespace JrzAsp.Lib.TypeUtilities {
    public static class TypesCache {
        private static Type[] _appDomainTypes;

        private static readonly ConcurrentDictionary<string, Type> _nameToTypes =
            new ConcurrentDictionary<string, Type>();

        public static Type[] AppDomainTypes {
            get {
                if (_appDomainTypes != null) return _appDomainTypes;
                _appDomainTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(asm => asm.GetTypes()).ToArray();
                return _appDomainTypes;
            }
        }

        public static Type GetTypeByFullName(string fullName) {
            Type t;
            if (_nameToTypes.TryGetValue(fullName, out t)) return t;

            t = AppDomainTypes.First(x => x.FullName == fullName);
            if (t.FullName != null) {
                _nameToTypes[t.FullName] = t;
            }
            return t;
        }
    }
}