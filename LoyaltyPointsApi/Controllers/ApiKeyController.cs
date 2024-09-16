using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.RequestModels;
using LoyaltyPointsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyPointsApi.Controllers
{
    [ApiController]
    [Route("api/login")]
    public class ApiKeyController(IApiKeyService service) : ControllerBase
    {
        [HttpPost]

        public async Task<ActionResult> CreateApiKey(LoginRequestBody requestBody)
        {
            return Ok(await service.CreateApiKey(requestBody.RestaurantId)); 
        }
    }
}