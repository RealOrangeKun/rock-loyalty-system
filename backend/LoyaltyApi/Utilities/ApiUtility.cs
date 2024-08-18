using System.Text;
using System.Text.Json;
using LoyaltyApi.Config;
using Microsoft.Extensions.Options;

namespace LoyaltyApi.Utilities
{
    public class ApiUtility(IOptions<API> apiOptions)
    {
        public async Task<string> GetApiKey(string restaurantId)
        {
            using HttpClient client = new();
            var body = new
            {
                Acc = restaurantId ?? "600", // change acc for different restaurants
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