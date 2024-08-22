using System.Text;
using System.Text.Json;
using LoyaltyApi.Config;
using LoyaltyApi.Models;
using Microsoft.Extensions.Options;

namespace LoyaltyApi.Utilities
{
    public class ApiUtility(IOptions<API> apiOptions)
    {
        public async Task<string> GetApiKey(string restaurantId)
        {
            using HttpClient client = new();
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isDevelopment = string.Equals(envName, "Development", StringComparison.OrdinalIgnoreCase);
            var body = new
            {
                Acc = isDevelopment ? "600" : restaurantId ?? throw new ArgumentException("Resturant ID is missing"), // change acc for different restaurants
                UsrId = apiOptions.Value.UserId,
                Pass = apiOptions.Value.Password,
                Lng = "ENG",
                SrcApp = "WEBAPP",
                SrcVer = 1.00

            };
            string jsonBody = JsonSerializer.Serialize(body);
            StringContent content = new(jsonBody, Encoding.UTF8, "application/json");
            var result = await client.PostAsync("http://192.168.1.11:5000/api/chkusr", content);
            return await result.Content.ReadAsStringAsync();
        }
        public async Task<string> GenerateVoucher(Voucher voucher, string apiKey)
        {
            using HttpClient client = new();
            var body = new
            {
                DTL = new[]
                {
                    new
                        {
                            VOCHNO = 1,// This is constant do not change it
                            VOCHVAL = voucher.Value
                        }
                },
                CNO = voucher.CustomerId.ToString(),
                CCODE = "C"
            };
            string jsonBody = JsonSerializer.Serialize(voucher);
            StringContent content = new(jsonBody, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("XApiKey", apiKey);
            var result = await client.PostAsync($"{apiOptions.Value.BaseUrl}/api/HISCMD/ADDVOC", content);
            return await result.Content.ReadAsStringAsync();
        }
        public async Task<User> GetUserAsync(string? email, string? phoneNumber, int restaurantId, string apiKey)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Add("XApiKey", apiKey);
            var result = await client.GetAsync($"{apiOptions.Value.BaseUrl}/api/concmd/GETCON/C/${phoneNumber ?? email}");
            var json = await result.Content.ReadAsStringAsync();
            var userJson = JsonSerializer.Deserialize<JsonElement>(json);
            User? user = new()
            {
                Id = userJson.GetProperty("CNO").GetInt32(),                   // Mapping "CNO" to User.Id
                PhoneNumber = userJson.GetProperty("TEL1").GetString()!,       // Mapping "TEL1" to User.PhoneNumber
                Email = userJson.GetProperty("EMAIL").GetString()!,            // Mapping "EMAIL" to User.Email
                Name = userJson.GetProperty("CNAME").GetString()!,             // Mapping "CNAME" to User.Name
                RestaurantId = restaurantId,                                   // Use the passed restaurantId
                Password = userJson.GetProperty("PASS").GetString()!,
            };
            return user;

        }
    }
}