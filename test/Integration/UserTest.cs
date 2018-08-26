using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using microblogApi.Test.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace microblogApi.Test.Integration {
    public class UserTests : IClassFixture<MicroblogWebApplicationFactory> {
        readonly MicroblogWebApplicationFactory _factory;
        static Regex idfinder = new Regex(@".*id['""]\s*?:\s*?['""](\d+)");

        static StringContent JsonContent(string body)
            => new StringContent(body, Encoding.UTF8, "application/json");

        string GetFirstId(string str) => idfinder.Match(str).Groups[0].Value;

        public UserTests(MicroblogWebApplicationFactory factory) {
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
        [InlineData("{'username':'better'}")]
        [InlineData("{'email':'new@email.org'}")]
        [InlineData("{'password':'Fo0B@r2'}")]
        public async void TestUpdateUserValid(string json) {
            var id = UserFixtures.Betty.Id;
            var endpoint = $"/api/users/{id}";
            var client = _factory.CreateDefaultClient();

            var response = await client.PatchAsync(endpoint, JsonContent(json));
            Assert.True(response.IsSuccessStatusCode);
            response = await client.GetAsync(endpoint);

            var change = JObject.Parse(json).Properties().First();
            var result = JObject.Parse(response.Content.ReadAsStringAsync().Result);
            Assert.True(result.Property(change.Name).Value.ToString() == (string)change.Value);
        }

        [Fact]
        public void TestUpdateUserPassword() {
            throw new NotImplementedException();
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