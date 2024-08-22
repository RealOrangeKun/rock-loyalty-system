using System.Text;
using System.Text.Json;
using LoyaltyApi.Config;
using LoyaltyApi.Data;
using LoyaltyApi.Models;
using LoyaltyApi.Utilities;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;

namespace LoyaltyApi.Repositories
{
    public class UserRepository(IOptions<API> apiOptions, ApiUtility apiUtility) : IUserRepository
    {
        public async Task<object> CreateUserAsync(User user)
        {
            HttpClient client = new();
            var body = new
            {
                CCODE = "C", // Constant For Customer 
                CNO = "0", // 0 if New Customer else Check The Cno if Exist Update Else Insert
                CNAME = user.Name,
                FORDES = user.Name, // foreign desc
                // PASS = user.Password, // password
                TEL1 = user.PhoneNumber,
                TEL2 = "",
                EMAIL = user.Email,
                EMAIL1 = "",
            };
            // Serialize the body object to JSON
            string jsonBody = JsonSerializer.Serialize(body);

            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isDevelopment = string.Equals(envName, "Development", StringComparison.OrdinalIgnoreCase);
            var apiKey = await apiUtility.GetApiKey(isDevelopment ? "600" : user.RestaurantId.ToString() ?? throw new ArgumentException("RestaurantId cannot be null"));

            client.DefaultRequestHeaders.Add("XApiKey", apiKey);

            // Create a StringContent object with the JSON and specify the content type
            StringContent content = new(jsonBody, Encoding.UTF8, "application/json");

            // Send the POST request
            HttpResponseMessage response = await client.PostAsync($"{apiOptions.Value.BaseUrl}/api/CONCMD/ADDCON", content);

            // Check the response status and handle accordingly
            string message = await response.Content.ReadAsStringAsync();
            if (message.Replace(" ", "").Contains("ERR"))
                throw new HttpRequestException($"Request to create user failed with message: {message}");
            return true;
        }

        public async Task<User> GetUserAsync(string? email, string? phoneNumber, int restaurantId)
        {
            var apiKey = await apiUtility.GetApiKey(restaurantId.ToString());

            return await apiUtility.GetUserAsync(email, phoneNumber, restaurantId, apiKey);

        }
    }
}