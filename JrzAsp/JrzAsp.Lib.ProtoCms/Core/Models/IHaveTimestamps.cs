using System;

namespace JrzAsp.Lib.ProtoCms.Core.Models {
    public interface IHaveTimestamps {
        DateTime CreatedUtc { get; set; }
        DateTime UpdatedUtc { get; set; }
    }
}