using System.ComponentModel.DataAnnotations;

namespace microblogApi.Models {
    public partial class User {
        public long Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string UserName { get; set; }
    }
}