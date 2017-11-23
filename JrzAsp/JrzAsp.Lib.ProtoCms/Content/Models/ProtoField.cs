using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JrzAsp.Lib.ProtoCms.Core.Models;

namespace JrzAsp.Lib.ProtoCms.Content.Models {
    public class ProtoField : IHaveTimestamps {

        public ProtoField() {
            Id = Guid.NewGuid().ToString();
            CreatedUtc = DateTime.UtcNow;
            UpdatedUtc = CreatedUtc;
        }

        [Required]
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string ContentId { get; set; }

        [Required]
        [MaxLength(64)]
        public string FieldName { get; set; }

        public string StringValue { get; set; }

        public decimal? NumberValue { get; set; }

        public bool? BooleanValue { get; set; }

        public DateTime? DateTimeValue { get; set; }

        [ForeignKey(nameof(ContentId))]
        public virtual ProtoContent Content { get; set; }

        [Required]
        public DateTime CreatedUtc { get; set; }

        [Required]
        public DateTime UpdatedUtc { get; set; }

        [Required]
        [MaxLength(128)]
        public string FieldClassTypeName { get; set; }
    }
}