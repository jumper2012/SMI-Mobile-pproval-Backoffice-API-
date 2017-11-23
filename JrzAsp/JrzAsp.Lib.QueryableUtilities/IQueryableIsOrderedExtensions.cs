using System.Linq;

namespace JrzAsp.Lib.QueryableUtilities {
    public static class IQueryableIsOrderedExtensions {
        public static bool IsOrdered<T>(this IQueryable<T> queryable) {
            return OrderingMethodFinder.OrderMethodExists(queryable.Expression);
        }
    }
}