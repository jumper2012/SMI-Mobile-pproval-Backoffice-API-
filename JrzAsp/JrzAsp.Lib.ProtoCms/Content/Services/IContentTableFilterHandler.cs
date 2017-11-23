using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Vue.Models;

namespace JrzAsp.Lib.ProtoCms.Content.Services {
    public interface IContentTableFilterHandler : IContentHandler {
        string Id { get; }
        string Name { get; }
        string Description { get; }

        bool CanFilter(ContentType contentType);

        ContentTableFilterForm BuildFilterForm(ContentType contentType);

        VueComponentDefinition[] FilterFormVues(ContentTableFilterForm filterForm, ContentType contentType);

        ContentTableFilterOperation[] SetupFilteringOperations(ContentTableFilterForm filterForm,
            ContentType contentType);
    }
}