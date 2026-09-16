using DTOs.Client;
using DTOs.Person;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Tests.Client
{
    public class ClientControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _httpClient;

        public ClientControllerTests(WebApplicationFactory<Program> factory)
        {
            // This simulates your React app making a 'fetch' call
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task RegisterClient_ShouldReturnBadRequest_WhenPersonDataIsInvalid()
        {
            // Arrange: Creating a request that fails STATIC validation (Empty First Name)
            var invalidRequest = new ClientAddRequest
            {
                Person = new PersonAddRequest
                {
                    FirstName = "", // Invalid!
                    NationalId = "N123",
                    Phone = "123456",
                    Gendor = 'M'
                }
            };

            var response = await _httpClient.PostAsJsonAsync("/api/client/create", invalidRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // 🏛️ Read as string first to avoid JsonException
            var content = await response.Content.ReadAsStringAsync();

            // If your filter returns a List<string>, keep this:
            // var errorList = JsonSerializer.Deserialize<List<string>>(content);
            // Assert.Contains("First Name is required.", errorList);

            // If your filter returns just a string, use this:
            Assert.Contains("First Name is required.", content);
        }
    }
}
