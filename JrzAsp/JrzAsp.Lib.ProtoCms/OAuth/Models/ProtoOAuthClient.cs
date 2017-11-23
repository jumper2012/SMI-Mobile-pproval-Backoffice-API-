using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using JrzAsp.Lib.ProtoCms.Core.Models;

namespace JrzAsp.Lib.ProtoCms.OAuth.Models {
    public class ProtoOAuthClient : IHaveTimestamps {

        public const string ALLOW_ALL_ORIGINS = "*";

        public ProtoOAuthClient() {
            Id = Guid.NewGuid().ToString();
            IsActive = true;
            CreatedUtc = DateTime.UtcNow;
            UpdatedUtc = CreatedUtc;
            AllowedOriginsCsv = ALLOW_ALL_ORIGINS;
        }

        [Key]
        [MaxLength(128)]
        public string Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string SecretHash { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        [Required]
        public ProtoOAuthApplicationType ApplicationType { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public int RefreshTokenLifetimeSeconds { get; set; }

        [Required]
        [MaxLength(512)]
        public string AllowedOriginsCsv { get; set; }

        [Required]
        public DateTime CreatedUtc { get; set; }

        [Required]
        public DateTime UpdatedUtc { get; set; }

        public static IEnumerable<string> GetAllowedOrigins(string allowedOriginsCsv) {
            var allowedOrigin = allowedOriginsCsv ?? ALLOW_ALL_ORIGINS;
            return allowedOrigin == ALLOW_ALL_ORIGINS
                ? new[] {ALLOW_ALL_ORIGINS}
                : allowedOrigin.Split(',', ';').Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim());
        }
    }
}