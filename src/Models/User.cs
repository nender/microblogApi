using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace microblogApi.Models {
    public partial class User : ITimestamps {
        [Key]
        public long Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string UserName { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public List<Micropost> Microposts { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}