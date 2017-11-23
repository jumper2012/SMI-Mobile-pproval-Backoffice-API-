using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Content.Models;

namespace JrzAsp.Lib.ProtoCms.Content.Services {
    public interface IContentWhereConditionsProvider : IGlobalSingletonDependency {
        decimal Priority { get; }
        IEnumerable<ContentWhereCondition> DefineWhereConditions();
    }
}