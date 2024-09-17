using LoyaltyPointsApi.RequestModels;
using LoyaltyPointsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyPointsApi.Controllers
{
    [ApiController]
    [Route("api/threshold")]
    public class ThresholdController(IThresholdService thresholdService) :ControllerBase
    {
        [HttpPost]
        [Route ("{RestaurantId}")]
        public async Task<ActionResult> AddThreshold([FromBody]ThresholdRequestModel thresholdRequestModel, [FromRoute] int RestaurantId)
        {
            try
            {
                await thresholdService.AddThreshold(thresholdRequestModel, RestaurantId);
               return StatusCode(201, new { success = true, message = "Threshold added" });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Route ("{RestaurantId}")]
        public async Task<ActionResult> GetRestaurantThresholds([FromRoute] int RestaurantId)
        {
            try
            {
                var result = await thresholdService.GetRestaurantThresholds(RestaurantId);
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
        [HttpGet]
        [Route ("get/{RestaurantId}")]
        public async Task<ActionResult> GetRestaurantThreshold([FromRoute] int RestaurantId, [FromQuery] string ThresholdName)
        {
            try
            {
                var result = await thresholdService.GetRestaurantThreshold(RestaurantId, ThresholdName);
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
        [Route ("{RestaurantId}")]
        public async Task<ActionResult> UpdateThreshold([FromBody]ThresholdRequestModel thresholdRequestModel, [FromRoute] int RestaurantId, [FromQuery] string ThresholdName)
        {
            try
            {
                var threshold = await thresholdService.GetRestaurantThreshold(RestaurantId, ThresholdName);
                if (threshold == null) return NotFound(new { success = false, message = "Threshold not found" });
                var result =  await thresholdService.UpdateThreshold(thresholdRequestModel, RestaurantId, ThresholdName);
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