using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LoyaltyApi.Config;
using LoyaltyApi.Models;
using Microsoft.Extensions.Options;
using Sprache;

namespace LoyaltyApi.Repositories
{
    public class UserRepository(IOptions<API> apiOptions) : IUserRepository
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
                TEL1 = user.PhoneNumber,
                TEL2 = "",
                EMAIL = user.Email,
                EMAIL1 = "",
            };
            // Serialize the body object to JSON
            string jsonBody = JsonSerializer.Serialize(body);

            var apiKey = await LoginToApi();

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

        public Task GetUserAsync(int userId, int restaurantId)
        {
            throw new NotImplementedException();
        }

        private async Task<string> LoginToApi()
        {
            using HttpClient client = new();
            var body = new
            {
                Acc = "600", // change acc for different restaurants
                UsrId = apiOptions.Value.UserId,
                Pass = apiOptions.Value.Password,
                Lng = "ENG",
                SrcApp = "WEBAPP",
                SrcVer = 1.00

            };
            string jsonBody = JsonSerializer.Serialize(body);
            StringContent content = new(jsonBody, Encoding.UTF8, "application/json");
            var result = await client.PostAsync("https://login.microsystems-eg.com/api/chkusr", content);
            return await result.Content.ReadAsStringAsync();
        }
    }
}