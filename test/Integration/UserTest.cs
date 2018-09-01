using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using microblogApi.Test.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace microblogApi.Test.Integration {
    public class UserTests {
        readonly HttpClient client;
        static Regex idfinder = new Regex(@".*id['""]\s*?:\s*?['""](\d+)");

        static StringContent JsonContent(string body)
            => new StringContent(body, Encoding.UTF8, "application/json");

        string GetFirstId(string str) => idfinder.Match(str).Groups[0].Value;

        public UserTests() {
            client = new MicroblogWebApplicationFactory().CreateDefaultClient();
        }

        public static async Task<bool> AuthenticateAsync(HttpClient client, object post) {
            var response = await client.PostAsJsonAsync("/api/authenticate", post);
            if (!response.IsSuccessStatusCode)
                return false;

            var token = await response.Content.ReadAsStringAsync();

            // strip quotes
            token = token.Substring(1, token.Length - 2);
            client.DefaultRequestHeaders.Add("Authorization", new[] {"Bearer " + token});
            return true;
        }

        [Fact]
        public async void TestCreateAndDestroyValidUser() {
            var post = new { email = "joe@schmo.net", username = "alibaba", password = "Fo0B@r" };
            var response = await client.PostAsJsonAsync("/api/users", post);
            Assert.True(response.IsSuccessStatusCode);
            var user = JObject.Parse(await response.Content.ReadAsStringAsync());

            bool authOK = await AuthenticateAsync(client, post);
            Assert.True(authOK);

            response = await client.DeleteAsync($"/api/users/{user["id"]}");
            Assert.True(response.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData("{email':null, 'username':null, 'password':null}")]
        [InlineData("{email':'', 'username':'', 'password':''}")]
        [InlineData("{email':'ok@okeydokey.net', 'username':'hellosir', 'password':'tooeasy'}")]
        [InlineData("{email':'notanemail', 'username':'hellosir', 'password':'Fo0B@r'}")]
        [InlineData("{email':'ok@okeydokey.net', 'username':'hellosir'}")]
        [InlineData("{'username':'hellosir'}")]
        [InlineData("")]
        public async void TestCreateUserInvalid(string json) {
            var response = await client.PostAsJsonAsync("/api/users", json);
            Assert.False(response.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData("{'username':'better'}")]
        [InlineData("{'email':'new@email.org'}")]
        public async void TestUpdateUser(string json) {
            var betty = UserFixtures.Betty;

            var authOK = await AuthenticateAsync(client, new {email = betty.Email, password = "Fo0b@r"});
            Assert.True(authOK);

            var endpoint = $"/api/users/{betty.Id}";
            var response = await client.PatchAsync(endpoint, JsonContent(json));
            Assert.True(response.IsSuccessStatusCode);
            response = await client.GetAsync(endpoint);

            var change = JObject.Parse(json).Properties().First();
            var result = JObject.Parse(await response.Content.ReadAsStringAsync());
            Assert.Equal(result.Property(change.Name).Value.ToString(), (string)change.Value);
        }

        [Fact]
        public async void TestUpdateUserPassword() {
            var betty = UserFixtures.Betty;
            var authOK = await AuthenticateAsync(client, new { email = betty.Email, password = "Fo0b@r" });
            Assert.True(authOK);

            var content = JsonContent("{'password':'Fo0b@r2'}");
            var result = await client.PatchAsync($"/api/users/{betty.Id}", content);
            Assert.True(result.IsSuccessStatusCode);

            result = await client.PostAsJsonAsync("/api/authenticate", new { email = betty.Email, password = "Fo0b@r2" });
            Assert.True(result.IsSuccessStatusCode);
        }
    }
}