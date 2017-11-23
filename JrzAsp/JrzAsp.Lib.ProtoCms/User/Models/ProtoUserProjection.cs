using System;

namespace JrzAsp.Lib.ProtoCms.User.Models {
    public sealed class ProtoUserProjection : IProtoUser {

        public ProtoUserProjection() { }

        public ProtoUserProjection(IProtoUser existing) {
            Id = existing.Id;
            UserName = existing.UserName;
            DisplayName = existing.DisplayName;
            IsActivated = existing.IsActivated;
            PasswordHash = existing.PasswordHash;
            PhotoUrl = existing.PhotoUrl;
            CreatedUtc = existing.CreatedUtc;
            UpdatedUtc = existing.UpdatedUtc;
        }

        public string Id { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public bool IsActivated { get; set; }
        public string PasswordHash { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
    }
}