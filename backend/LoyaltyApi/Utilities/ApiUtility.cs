using System.Security.Cryptography;
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
            string jsonBody = JsonSerializer.Serialize(body);
            StringContent content = new(jsonBody, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("XApiKey", apiKey);
            var result = await client.PostAsync($"{apiOptions.Value.BaseUrl}/api/HISCMD/ADDVOC", content);
            var message = await result.Content.ReadAsStringAsync();
            if (message.Replace(" ", "").Contains("ERR"))
                throw new HttpRequestException($"Request to create user failed with message: {message}");
            string responseContent = await result.Content.ReadAsStringAsync();

<<<<<<< HEAD
        string responseContent = await result.Content.ReadAsStringAsync();

        var responseObject = JsonSerializer.Deserialize<List<String>>(responseContent);

        
        return responseObject.First();

    
=======
            List<string>? responseObject = JsonSerializer.Deserialize<List<string>>(responseContent) ?? throw new HttpRequestException("Response object is null");
            return responseObject.First();
>>>>>>> 88bf45655491a787220906495cbd1b247ba75c1c
        }
        public async Task<User?> GetUserAsync(User user, string apiKey)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Add("XApiKey", apiKey);
            string url = $"{apiOptions.Value.BaseUrl}/api/concmd/GETCON/C/{user.PhoneNumber ?? user.Email ?? throw new ArgumentException("Phone number or email is missing")}";
            var result = await client.GetAsync(url);
            var json = await result.Content.ReadAsStringAsync();
            if (json.ToString().Replace(" ", "").Contains("ERR")) throw new HttpRequestException("User not found");
            var userJson = JsonSerializer.Deserialize<JsonElement>(json);
            User? createdUser = new()
            {
                Id = userJson.GetProperty("CNO").GetInt32(),                   // Mapping "CNO" to User.Id
                PhoneNumber = userJson.GetProperty("TEL1").GetString()!,       // Mapping "TEL1" to User.PhoneNumber
                Email = userJson.GetProperty("EMAIL").GetString()!,            // Mapping "EMAIL" to User.Email
                Name = userJson.GetProperty("CNAME").GetString()!,             // Mapping "CNAME" to User.Name
                RestaurantId = user.RestaurantId,                                   // Use the passed restaurantId
                // Password = userJson.GetProperty("PASS").GetString()!,
            };
            return createdUser;

        }
        public async Task<User?> CreateUserAsync(User user, string apiKey)
        {
            using HttpClient client = new();
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

            client.DefaultRequestHeaders.Add("XApiKey", apiKey);

            // Create a StringContent object with the JSON and specify the content type
            StringContent content = new(jsonBody, Encoding.UTF8, "application/json");

            // Send the POST request
            HttpResponseMessage response = await client.PostAsync($"{apiOptions.Value.BaseUrl}/api/CONCMD/ADDCON", content);

            // Check the response status and handle accordingly
            string message = await response.Content.ReadAsStringAsync();
            if (message.Replace(" ", "").Contains("ERR"))
                throw new HttpRequestException($"Request to create user failed with message: {message}");

            return user;
        }
    }
}