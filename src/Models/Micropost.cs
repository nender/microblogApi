using System.ComponentModel.DataAnnotations;

namespace microblogApi.Models {
    public class Micropost {
        [Key]
        public long Id { get;set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public long UserId { get; set; }

        public User User { get; set; }
    }
}