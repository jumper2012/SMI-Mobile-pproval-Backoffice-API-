namespace JrzAsp.Lib.ProtoCms.Content.Services {
    public interface IContentHandler : IPerRequestDependency {
        string[] HandledContentTypeIds { get; }
        decimal Priority { get; }
    }
}