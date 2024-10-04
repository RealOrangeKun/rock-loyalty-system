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
    public class LoyaltyPointsController(ILoyaltyPointsTransactionService service,
    ILogger<LoyaltyPointsController> logger) : ControllerBase
    {
        [HttpGet]
        [Route("customers/{customerId}/restaurants/{restaurantId}")]
        public async Task<ActionResult> GetTotalPoints([FromRoute] int customerId, [FromRoute] int restaurantId)
        {
            logger.LogInformation("Request to get total points: {customerId} for restaurant: {restaurantId}", customerId, restaurantId);
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
                logger.LogError(ex, "Error getting total points: {customerId} for restaurant: {restaurantId}", customerId, restaurantId);
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}