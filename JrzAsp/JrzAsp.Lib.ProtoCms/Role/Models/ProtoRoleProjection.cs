using System;

namespace JrzAsp.Lib.ProtoCms.Role.Models {
    public sealed class ProtoRoleProjection : IProtoRole {

        public ProtoRoleProjection() { }

        public ProtoRoleProjection(IProtoRole existing) {
            Id = existing.Id;
            Name = existing.Name;
            Description = existing.Description;
            CreatedUtc = existing.CreatedUtc;
            UpdatedUtc = existing.UpdatedUtc;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
    }
}