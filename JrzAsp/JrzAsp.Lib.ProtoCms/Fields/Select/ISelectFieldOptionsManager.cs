namespace JrzAsp.Lib.ProtoCms.Fields.Select {
    public interface ISelectFieldOptionsManager : IPerRequestDependency {
        ISelectFieldOptionsHandler[] Handlers { get; }
        ISelectFieldOptionsHandler GetHandler(string id);
    }
}