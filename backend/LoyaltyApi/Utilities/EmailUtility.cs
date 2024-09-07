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
    public class EmailUtility(IOptions<EmailOptions> emailOptions,
    ILogger<EmailUtility> logger)
    {
        public async Task SendEmailAsync(string email, string subject, string message, string name)
        {
            using HttpClient client = new();
            var body = new
            {
                recipient = email,
                senderName = name,
                subject,
                body = message
            };

            string jsonBody = JsonSerializer.Serialize(body);

            StringContent content = new(jsonBody, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", emailOptions.Value.Key);

            // Send the POST request
            var result = await client.PostAsync($"{emailOptions.Value.BaseUrl}/api/email", content);

            // Log the status code
            logger.LogInformation("Request made to send email. Response Status Code: {statusCode}", result.StatusCode);

            // Optionally read and log the response content
            var responseContent = await result.Content.ReadAsStringAsync();

            // Ensure the request was successful
            result.EnsureSuccessStatusCode();
        }
    }
}