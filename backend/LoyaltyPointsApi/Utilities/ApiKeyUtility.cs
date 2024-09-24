
using System.Text;
using System.Text.Json;
using LoyaltyPointsApi.Config;
using LoyaltyPointsApi.Models;
using Microsoft.Extensions.Options;

namespace LoyaltyPointsApi.Utilities
{
    public class ApiUtility(IOptions<ApiOptions> apiOptions,
    ILogger<ApiUtility> logger)
    {
        public async Task<string> GetApiKey(string restaurantId)
        {
            using HttpClient client = new();
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isDevelopment = string.Equals(envName, "Development", StringComparison.OrdinalIgnoreCase);
            var isTesting = string.Equals(envName, "Testing", StringComparison.OrdinalIgnoreCase);
            var body = new
            {
                Acc = isDevelopment || isTesting ? "600" : restaurantId ?? throw new ArgumentException("Resturant ID is missing"), // change acc for different restaurants
                UsrId = apiOptions.Value.UserId,
                Pass = apiOptions.Value.Password,
                Lng = "ENG",
                SrcApp = "WEBAPP",
                SrcVer = 1.00

            };
            string jsonBody = JsonSerializer.Serialize(body);
            StringContent content = new(jsonBody, Encoding.UTF8, "application/json");
            var result = await client.PostAsync("http://192.168.1.11:5000/api/chkusr", content);
            logger.LogInformation("Request made to get ApiKey. Response Status Code: {statusCode}", result.StatusCode);
            return await result.Content.ReadAsStringAsync();
        }
        public async Task<User?> GetUserAsync(User user, string apiKey)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Add("XApiKey", apiKey);
            string url = $"{apiOptions.Value.BaseUrl}/api/concmd/GETCON/C/{user.PhoneNumber ?? user.Email ?? user.CustomerId.ToString() ?? throw new ArgumentException("Phone number or email is missing")}";
            var result = await client.GetAsync(url);
            var json = await result.Content.ReadAsStringAsync();
            if (json.ToString().Replace(" ", "").Contains("ERR")) return null;
            var userJson = JsonSerializer.Deserialize<JsonElement>(json);
            User? createdUser = new()
            {
                CustomerId = userJson.GetProperty("CNO").GetInt32(),                   // Mapping "CNO" to User.Id
                PhoneNumber = userJson.GetProperty("TEL1").GetString()!,       // Mapping "TEL1" to User.PhoneNumber
                Email = userJson.GetProperty("EMAIL").GetString()!,            // Mapping "EMAIL" to User.Email
                Name = userJson.GetProperty("CNAME").GetString()!,             // Mapping "CNAME" to User.Name
                RestaurantId = user.RestaurantId,                                   // Use the passed restaurantId
            };
            logger.LogInformation("Request made to get user. Response Message: {message}", json);
            return createdUser;
        }
        
    }
}