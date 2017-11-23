using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JrzAsp.Lib.ProtoCms.Permission.Models {
    public class ProtoPermissionsMap {
        private const string UQ_INDEX = "UQ_PermissionsMap";

        public ProtoPermissionsMap() {
            Id = Guid.NewGuid().ToString();
        }

        [Required]
        [Key]
        public string Id { get; set; }

        [Required]
        [MaxLength(128)]
        [Index(UQ_INDEX, 1, IsUnique = true)]
        public string RoleName { get; set; }

        [Required]
        [MaxLength(128)]
        [Index(UQ_INDEX, 2, IsUnique = true)]
        public string PermissionId { get; set; }

        [Required]
        public bool HasPermission { get; set; }
    }
}