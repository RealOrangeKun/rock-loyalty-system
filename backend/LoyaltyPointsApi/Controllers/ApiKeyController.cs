using LoyaltyPointsApi.Config;
using LoyaltyPointsApi.Models;
using LoyaltyPointsApi.RequestModels;
using LoyaltyPointsApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LoyaltyPointsApi.Controllers
{
    /// <summary>
    /// API Key
    /// </summary>
    [ApiController]
    [Route("api/api-key")]
    public class ApiKeyController(
        IApiKeyService service,
        IOptions<AdminOptions> adminOptions,
        ILogger<ApiKeyController> logger) : ControllerBase
    {
        /// <summary>
        /// Creates a new ApiKey for the specified restaurantId
        /// </summary>
        /// <param name="requestBody">The request body containing the username and password of the admin as well as the restaurantId</param>
        /// <returns>A response with the generated ApiKey</returns>
        /// <response code="201">ApiKey created successfully</response>
        /// <response code="401">Unauthorized, invalid username or password</response>
        /// <example>
        /// POST /api/api-key
        /// {
        ///     "username": "admin",
        ///     "password": "password",
        ///     "restaurantId": 1
        /// }
        /// </example>
        /// <example>
        /// HTTP/1.1 201 Created
        /// {
        ///     "success": true,
        ///     "message": "Api key created successfully",
        ///     "data": {
        ///         "apiKey": "dfsfdsdfsdfds"
        ///     }
        /// }
        /// </example>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateApiKey(LoginRequestBody requestBody)
        {
            try
            {
                if (requestBody.Username != adminOptions.Value.Username ||
                requestBody.Password != adminOptions.Value.Password) return Unauthorized();
                logger.LogInformation("Creating ApiKey for restaurantId: {restaurantId}", requestBody.RestaurantId);
                ApiKey apiKey = await service.CreateApiKey(requestBody.RestaurantId);
                return StatusCode(StatusCodes.Status201Created, new
                {
                    success = true,
                    message = "Api key created successfully",
                    data = new { apiKey = apiKey.Key }
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating ApiKey for restaurantId: {restaurantId}", requestBody.RestaurantId);
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}
