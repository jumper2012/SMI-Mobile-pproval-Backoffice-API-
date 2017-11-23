using System;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Vue.Models;

namespace JrzAsp.Lib.ProtoCms.Datum.Services {
    public interface IDatumTableFilterHandler : IDatumHandler {
        string Id { get; }
        string Name { get; }
        string Description { get; }
        
        ContentTableFilterForm BuildFilterForm(Type datumType);
        VueComponentDefinition[] FilterFormVues(ContentTableFilterForm filterForm, Type datumType);
        ContentTableFilterOperation[] SetupFilteringOperations(ContentTableFilterForm filterForm, Type datumType);
    }
}