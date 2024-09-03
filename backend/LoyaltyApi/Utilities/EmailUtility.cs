using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LoyaltyApi.Config;
using Microsoft.Extensions.Options;

namespace LoyaltyApi.Utilities
{
    public class EmailUtility(IOptions<EmailOptions> emailOptions)
    {
        public async Task SendEmailAsync(string email, string subject, string message, string name)
        {
            using HttpClient client = new();
            var body = new
            {
                recipient = email,
                name,
                subject,
                body = message
            };

            string jsonBody = JsonSerializer.Serialize(body);

            StringContent content = new(jsonBody, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", emailOptions.Value.Key);

            var result = await client.PostAsync($"{emailOptions.Value.BaseUrl}/api/email", content);
        }
    }
}