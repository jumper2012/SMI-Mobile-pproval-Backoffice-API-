using System;
using System.Linq;
using System.Linq.Expressions;

namespace JrzAsp.Lib.QueryableUtilities {
    public static class IQueryableAddOrderByExtensions {
        public static IOrderedQueryable<T> AddOrderBy<T, TKey>(this IQueryable<T> q,
            Expression<Func<T, TKey>> orderExpression, bool isDescending = false) {
            var hasBeenOrdered = q.IsOrdered();

            if (hasBeenOrdered && q is IOrderedQueryable<T> oq) {
                return AddOrderBy(oq, orderExpression, isDescending);
            }
            return !isDescending ? q.OrderBy(orderExpression) : q.OrderByDescending(orderExpression);
        }

        public static IOrderedQueryable<T> AddOrderBy<T, TKey>(this IOrderedQueryable<T> q,
            Expression<Func<T, TKey>> orderExpression, bool isDescending = false) {
            var hasBeenOrdered = q.IsOrdered();

            if (hasBeenOrdered) return !isDescending ? q.ThenBy(orderExpression) : q.ThenByDescending(orderExpression);
            return !isDescending ? q.OrderBy(orderExpression) : q.OrderByDescending(orderExpression);
        }
    }
}