using System.Text;
using System.Text.Json;
using LoyaltyPointsApi.Config;
using Microsoft.Extensions.Options;

namespace LoyaltyPointsApi.Utilities
{
    public class EmailUtility(IOptions<EmailOptions> emailOptions,
    ILogger<EmailUtility> logger)
    {
        public async Task<bool> SendEmailAsync(string email, string subject, string message, string name)
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
            try
            {
                // Send the POST request
                var result = await client.PostAsync($"{emailOptions.Value.BaseUrl}/api/email", content);

                // Log the status code
                logger.LogInformation("Request made to send email. Response Status Code: {statusCode}", result.StatusCode);

                // Ensure the request was successful
                result.EnsureSuccessStatusCode();

                // Return true if the request was successful
                return true;
            }
            catch (HttpRequestException ex)
            {
                // Log the error and return false if the email failed to send
                logger.LogError(ex, "Failed to send email to {email}", email);
                return false;
            }
        }
    }
}