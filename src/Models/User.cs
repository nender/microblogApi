using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace microblogApi.Models {
    public class User : IdentityUser<long> {
    }
}