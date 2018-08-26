using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace microblogApi.Models {
    [ModelMetadataType(typeof(UserMetadata))]
    public partial class User : IdentityUser<long> {
    }

    public class UserMetadata {
        [Required]
        [EmailAddress]
        public virtual string Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public virtual string UserName { get; set; }
    }
}