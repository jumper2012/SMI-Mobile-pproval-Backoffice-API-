using System;
using System.Collections.Generic;

namespace JrzAsp.Lib.QueryableUtilities {
    public class ObjectComparer<T> : IEqualityComparer<T> where T : class {

        public ObjectComparer(Func<T, object> expr) {
            _expr = expr;
        }

        private Func<T, object> _expr { get; }

        public bool Equals(T x, T y) {
            var first = _expr.Invoke(x);
            var second = _expr.Invoke(y);
            return first != null && first.Equals(second);

        }

        public int GetHashCode(T obj) {
            return obj.GetHashCode();
        }
    }

    public static class ObjectComparer {
        public static ObjectComparer<T> For<T>(Func<T, object> expr) where T : class {
            return new ObjectComparer<T>(expr);
        }
    }
}