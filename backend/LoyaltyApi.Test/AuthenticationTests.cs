using System.Net;
using System.Text;
using System.Text.Json;
using DotNetEnv;

namespace LoyaltyApi.Test;

public class AuthenticationTests
{
    private readonly HttpClient httpClient = new();
    public AuthenticationTests()
    {
        Env.Load();
        var baseUrl = Environment.GetEnvironmentVariable("BASE_URL");
        httpClient.BaseAddress = new Uri(baseUrl ?? throw new ArgumentException("BASE_URL is missing"));
    }
    [Fact]
    public async Task Login_With_Invalid_Credentials()
    {
        // Arrange
        var loginBody = new
        {
            Email = "test",
            Password = "test",
            RestaurantId = 1
        };
        // Act
        var response = await httpClient.PostAsync("/api/auth/login", new StringContent(JsonSerializer.Serialize(loginBody), Encoding.UTF8, "application/json"));

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    [Fact]
    public async Task Login_With_Valid_Credentials()
    {
        // Arrange
        var loginBody = new
        {
            Email = "test@test.com",
            Password = "test",
            RestaurantId = 1
        };
        // Act
        var response = await httpClient.PostAsync("/api/auth/login", new StringContent(JsonSerializer.Serialize(loginBody), Encoding.UTF8, "application/json"));
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotNull(content);
        Assert.NotNull(response.Headers.GetValues("Set-Cookie"));
        Assert.Single(response.Headers.GetValues("Set-Cookie"));
        Assert.NotNull(response.Headers.GetValues("Set-Cookie")?.FirstOrDefault(x => x.Contains("refreshToken")));
    }
    [Fact]
    public async Task Register_With_Valid_Credentials()
    {
        // Arrange
        var registerBody = new
        {
            Email = "test@test.com",
            Password = "test",
            PhoneNumber = "1234567890",
            Name = "test",
            RestaurantId = 1
        };

        // Act
        var response = await httpClient.PostAsync("/api/user", new StringContent(JsonSerializer.Serialize(registerBody), Encoding.UTF8, "application/json"));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}