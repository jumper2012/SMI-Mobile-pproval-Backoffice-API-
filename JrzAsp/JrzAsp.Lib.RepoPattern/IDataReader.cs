namespace JrzAsp.Lib.RepoPattern {

    public interface IDataReader<TData, out TDataFluentQuery>
        where TData : class
        where TDataFluentQuery : IDataFluentQuery<TData, TDataFluentQuery> {

        TDataFluentQuery FluentQuery();
    }
}