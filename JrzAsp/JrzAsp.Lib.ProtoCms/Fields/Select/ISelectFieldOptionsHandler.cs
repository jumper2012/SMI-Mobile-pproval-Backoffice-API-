using JrzAsp.Lib.QueryableUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.Select {
    public interface ISelectFieldOptionsHandler : IPerRequestDependency {
        string Id { get; }
        string DevDescription { get; }
        decimal Priority { get; }
        PaginatedQueryable<object> GetOptions(string keywords, int page, int limitPerPage, string handlerParam);
        object GetOptionObject(string optionValue, string handlerParam);
        SelectFieldOption GetOptionDisplay(object option, string handlerParam);
    }
}