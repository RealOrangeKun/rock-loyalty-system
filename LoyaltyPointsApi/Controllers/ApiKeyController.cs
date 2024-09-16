using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Config;
using LoyaltyPointsApi.Models;
using LoyaltyPointsApi.RequestModels;
using LoyaltyPointsApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LoyaltyPointsApi.Controllers
{
    [ApiController]
    [Route("api/api-key")]
    public class ApiKeyController(IApiKeyService service,
    IOptions<AdminOptions> adminOptions) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> CreateApiKey(LoginRequestBody requestBody)
        {
            if (requestBody.Username != adminOptions.Value.Username ||
            requestBody.Password != adminOptions.Value.Password) return Unauthorized();
            ApiKey apiKey = await service.CreateApiKey(requestBody.RestaurantId);
            return Ok(apiKey.Key);
        }
    }
}