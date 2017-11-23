using System.Linq;

namespace JrzAsp.Lib.RepoPattern {
    public interface IDataFluentQuery<out TData, out TDataFluentQuery>
        where TData : class
        where TDataFluentQuery : IDataFluentQuery<TData, TDataFluentQuery> {
        bool DefaultSortIsDescending { get; }

        IQueryable<TData> Query();
        TData FindById(object id);
        TDataFluentQuery All();
        TDataFluentQuery Search(string keywords);
        TDataFluentQuery Search(string column, string keywords);
        TDataFluentQuery Sort(bool isDescending);
        TDataFluentQuery Sort(string column, bool isDescending);
    }
}