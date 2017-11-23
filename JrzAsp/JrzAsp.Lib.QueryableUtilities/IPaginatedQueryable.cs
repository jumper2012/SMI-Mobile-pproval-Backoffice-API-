using System.Linq;

namespace JrzAsp.Lib.QueryableUtilities {
    public interface IPaginatedQueryable {
        int CurrentPage { get; }
        int TotalPage { get; }
        int Offset { get; }
        int Limit { get; }
        int Count { get; }
        int StartNumbering { get; }
        IQueryable CurrentPageQueryableBase { get; }
    }

    public interface IPaginatedQueryable<out TData> : IPaginatedQueryable {
        IQueryable<TData> CurrentPageQueryable { get; }
    }
}