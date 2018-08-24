using System.ComponentModel.DataAnnotations;

namespace microblogApi.Model {
    public class User {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }
    }
}