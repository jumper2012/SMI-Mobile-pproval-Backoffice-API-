using JrzAsp.Lib.ProtoCms.Core.Models;

namespace JrzAsp.Lib.ProtoCms.Role.Models {
    public interface IProtoRole : IHaveTimestamps {
        string Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
    }
}