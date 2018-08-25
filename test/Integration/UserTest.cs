using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Xunit;

namespace microblogApi.Test.Integration {
    public class UserTests : IClassFixture<TestingWebApplicationFactory<microblogApi.Startup>> {
        readonly TestingWebApplicationFactory<microblogApi.Startup> _factory;

        public UserTests(TestingWebApplicationFactory<microblogApi.Startup> factory) {
            _factory = factory;
        }

        [Fact]
        public async void TestCreateAndDestroyValidUser() {
            var x = new { email = "joe@schmo.net", username = "alibaba", password = "Fo0B@r" };
            var client = _factory.CreateDefaultClient();
            var response = await client.PostAsync("/api/users", new StringContent(JsonConvert.SerializeObject(x), Encoding.UTF8 ,"application/json"));
            Assert.True(response.IsSuccessStatusCode);
            var id = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content.ReadAsStringAsync().Result)["id"];
            response = await client.DeleteAsync($"/api/users/{id}");
            Assert.True(response.IsSuccessStatusCode);
        }
    }
}