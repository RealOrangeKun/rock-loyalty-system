using System.Text;
using System.Text.Json;
using DotNetEnv;

namespace LoyaltyApi.Test;

public class AuthenticationTests
{
    private readonly HttpClient httpClient = new();
    private readonly string baseUrl = Environment.GetEnvironmentVariable("BASE_URL");
    public AuthenticationTests()
    {
        Env.Load();
        httpClient.BaseAddress = new Uri(baseUrl ?? throw new ArgumentException("BASE_URL is missing"));
    }
    [Fact]
    public void Login_With_Invalid_Credentials()
    {
        // Arrange
        var loginBody = new
        {
            Email = "test",
            Password = "test"
        };
        // Act
        var response = httpClient.PostAsync("/api/auth/login", new StringContent(JsonSerializer.Serialize(loginBody), Encoding.UTF8, "application/json")).Result;
    }
}