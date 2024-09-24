using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyPointsApi.Controllers
{
    [ApiController]
    [Route("api/loyalty-points")]
    public class LoyaltyPointsApiController (ILoyaltyPointsTransactionService service) : ControllerBase
    {
        [HttpGet]
        [Route("custoomers/{customerId}/restaurants/{restaurantId}")]
        public async Task<ActionResult> GetTotalPoints([FromRoute] int customerId, [FromRoute] int restaurantId)
        {
            try
            {
                var result = await service.GetTotalPoints(customerId, restaurantId);
                return Ok(new
                {
                    success = true,
                    message = "Total points found",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}