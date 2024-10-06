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
        /// <summary>
        /// Get the total points for a customer at a restaurant
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="restaurantId">The restaurant id</param>
        /// <returns>Total points for the customer at the restaurant</returns>
        /// <response code="200">Total points found</response>
        /// <response code="500">Internal server error</response>
        /// <example>
        ///  GET /api/loyalty-points/customers/1/restaurants/1 HTTP/1.1
        ///  Host: localhost:5000
        ///  Content-Type: application/json
        /// </example>
        /// <example>
        ///  HTTP/1.1 200 OK
        ///  Content-Type: application/json
        ///  
        ///  {
        ///      "success": true,
        ///      "message": "Total points found",
        ///      "Data": {
        ///          "TotalPoints": 100
        ///      }
        ///  }
        /// </example>
        [HttpGet]
        [Route("customers/{customerId}/restaurants/{restaurantId}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
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
                    Data = new
                    {
                        TotalPoints = result
                    }
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