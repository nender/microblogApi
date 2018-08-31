using System;

namespace microblogApi.Models {
    public interface ITimestamps {
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
    }
}