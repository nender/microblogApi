using System.Collections.Generic;
using microblogApi.Models;

namespace microblogApi.Test.Data {
    public static class UserFixtures {
        public static readonly User Jim = new User {
            Id = 1,
            UserName = "jim",
            Email = "jim@example.org",
        };

        public static readonly  User Betty = new User {
            Id = 2,
            UserName = "Betty",
            Email = "queenb@example.org",
        };

        public static readonly IEnumerable<User> Users = new List<User> { Jim, Betty };
    }
}