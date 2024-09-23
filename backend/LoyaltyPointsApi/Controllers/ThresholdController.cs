using LoyaltyPointsApi.RequestModels;
using LoyaltyPointsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyPointsApi.Controllers
{
    [ApiController]
    [Route("api/threshold")]
    public class ThresholdController(IThresholdService thresholdService) : ControllerBase
    {
        [HttpPost]

        public async Task<ActionResult> AddThreshold([FromBody] ThresholdRequestModel thresholdRequestModel)
        {
            try
            {
                await thresholdService.AddThreshold(thresholdRequestModel);
                return StatusCode(201, new { success = true, message = "Threshold added" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("{restaurantId}")]
        public async Task<ActionResult> GetRestaurantThresholds([FromRoute] int restaurantId)
        {
            try
            {
                var result = await thresholdService.GetRestaurantThresholds(restaurantId);
                if (result == null) return NotFound(new { success = false, message = "Threshold not found" });
                return Ok(new
                {
                    success = true,
                    message = "Thresholds found",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        [Route("restaurant/{restaurantId}")]
        public async Task<ActionResult> GetRestaurantThreshold([FromRoute] int restaurantId, [FromQuery] int thresholdId)
        {
            try
            {
                var result = await thresholdService.GetRestaurantThreshold(restaurantId, thresholdId);
                if (result == null) return NotFound(new { success = false, message = "Threshold not found" });
                return Ok(new
                {
                    success = true,
                    message = "Threshold found",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpPut]
        [Route("restaurant/{restaurantId}/id/{thresholdId}")]
        public async Task<ActionResult> UpdateThreshold([FromBody] ThresholdRequestModel thresholdRequestModel, [FromRoute] int restaurantId, [FromRoute] int thresholdId)
        {
            try
            {
                var threshold = await thresholdService.GetRestaurantThreshold(restaurantId, thresholdId);
                if (threshold == null) return NotFound(new { success = false, message = "Threshold not found" });
                var result = await thresholdService.UpdateThreshold(thresholdRequestModel, restaurantId, thresholdId);
                return Ok(new
                {
                    success = true,
                    message = "Threshold updated",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

    }
}