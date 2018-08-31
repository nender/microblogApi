using System;
using System.ComponentModel.DataAnnotations;

namespace microblogApi.Models {
    public class Micropost : ITimestamps {
        [Key]
        public long Id { get;set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public long UserId { get; set; }

        public User User { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}