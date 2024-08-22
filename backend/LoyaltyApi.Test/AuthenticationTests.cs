using System.Net;
using System.Text;
using System.Text.Json;
using DotNetEnv;
using Xunit.Abstractions;

namespace LoyaltyApi.Test;

public class AuthenticationTests
{
    private readonly HttpClient httpClient = new();
    private readonly string name;
    private readonly string email;
    private readonly ITestOutputHelper outputHelper;
    public AuthenticationTests(ITestOutputHelper outputHelper)
    {
        Env.Load();
        var baseUrl = Environment.GetEnvironmentVariable("BASE_URL");
        httpClient.BaseAddress = new Uri(baseUrl ?? throw new ArgumentException("BASE_URL is missing"));
        email = $"{Guid.NewGuid().ToString()[..8]}@{Guid.NewGuid().ToString()[..8]}.com";
        name = Guid.NewGuid().ToString()[..8];
        this.outputHelper = outputHelper;
    }

    [Fact]
    public async Task Register()
    {
        // Arrange
        var registerBody = new
        {
            email,
            password = "test",
            phoneNumber = "1234567890",
            name,
            restaurantId = 1
        };
        // Act
        var response = await httpClient.PostAsync("/api/user", new StringContent(JsonSerializer.Serialize(registerBody), Encoding.UTF8, "application/json"));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
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
            Email = email,
            Password = "test",
            RestaurantId = 1
        };
        // Act

        var response = await httpClient.PostAsync("/api/auth/login", new StringContent(JsonSerializer.Serialize(loginBody), Encoding.UTF8, "application/json"));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotNull(content);
        Assert.NotNull(response.Headers.GetValues("Set-Cookie"));
        Assert.Single(response.Headers.GetValues("Set-Cookie"));
        Assert.NotNull(response.Headers.GetValues("Set-Cookie")?.FirstOrDefault(x => x.Contains("refreshToken")));
    }

}