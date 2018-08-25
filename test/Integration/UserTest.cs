using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace microblogApi.Test.Integration {
    public class UserTests : IClassFixture<TestingWebApplicationFactory<microblogApi.Startup>> {
        readonly TestingWebApplicationFactory<microblogApi.Startup> _factory;
        static Regex idfinder = new Regex(@".*id['""]\s*?:\s*?['""](\d+)");

        string GetFirstId(string str) => idfinder.Match(str).Groups[0].Value;

        public UserTests(TestingWebApplicationFactory<microblogApi.Startup> factory) {
            _factory = factory;
        }

        [Fact]
        public async void TestCreateAndDestroyValidUser() {
            var x = new { email = "joe@schmo.net", username = "alibaba", password = "Fo0B@r" };
            var client = _factory.CreateDefaultClient();
            var response = await client.PostAsJsonAsync("/api/users", x);
            Assert.True(response.IsSuccessStatusCode);
            var id = GetFirstId(response.Content.ReadAsStringAsync().Result);
            response = await client.DeleteAsync($"/api/users/{id}");
            Assert.True(response.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("tooeasy")]
        public async void TestCreateUserInvalidPassword(string pw) {
            var x = new { email = "joe@schmo.net", username = "alibaba", password = pw };
            var client = _factory.CreateDefaultClient();
            var response = await client.PostAsJsonAsync("/api/users", x);
            Assert.False(response.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("notanemail.com")]
        public async void TestCreateUserInvalidEmail(string email) {
            var x = new { email = email, username = "alibaba", password = "Fo0b@r" };
            var client = _factory.CreateDefaultClient();
            var response = await client.PostAsJsonAsync("/api/users", x);
            Assert.False(response.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("2sht")]
        [InlineData("waaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaytooooooolooooooooong")]
        public async void TestCreateUserInvalidUsername(string username) {
            var x = new { email = "joe@schmo.net", username = username, password = "Fo0b@r" };
            var client = _factory.CreateDefaultClient();
            var response = await client.PostAsJsonAsync("/api/users", x);
            Assert.False(response.IsSuccessStatusCode);
        }

        [Fact]
        public async void TestUserIndexAndIndividualUser() {
            var client = _factory.CreateDefaultClient();
            var response = await client.GetAsync("/api/users");
            Assert.True(response.IsSuccessStatusCode);
            var id = GetFirstId(response.Content.ReadAsStringAsync().Result);
            response = await client.GetAsync($"/api/users{id}");
            Assert.True(response.IsSuccessStatusCode);
        }
    }
}