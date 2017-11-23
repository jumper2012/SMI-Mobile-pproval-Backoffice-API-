using JrzAsp.Lib.ProtoCms.Core.Models;

namespace JrzAsp.Lib.ProtoCms.User.Models {
    public interface IProtoUser : IHaveTimestamps {
        string Id { get; set; }
        string UserName { get; set; }
        string DisplayName { get; set; }
        bool IsActivated { get; set; }
        string PasswordHash { get; set; }
        string PhotoUrl { get; set; }
    }
}