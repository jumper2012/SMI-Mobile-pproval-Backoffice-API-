using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JrzAsp.Lib.ProtoCms.OAuth.Models {
    public class ProtoOAuthRefreshToken {
        private const string UQ_INDEX = "UQ_Subject_ClientId";

        [Key]
        [MaxLength(128)]
        public string Id { get; set; }

        [Required]
        [MaxLength(128)]
        [Index(UQ_INDEX, 1, IsUnique = true)]
        public string Subject { get; set; }

        [Required]
        [MaxLength(128)]
        [Index(UQ_INDEX, 2, IsUnique = true)]
        public string ClientId { get; set; }

        [Required]
        public DateTime IssuedUtc { get; set; }

        [Required]
        public DateTime ExpiresUtc { get; set; }

        [Required]
        public string ProtectedTicket { get; set; }

        [ForeignKey(nameof(ClientId))]
        public virtual ProtoOAuthClient OAuthClient { get; set; }
    }
}