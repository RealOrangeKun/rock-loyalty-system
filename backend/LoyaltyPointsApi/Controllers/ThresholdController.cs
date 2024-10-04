using LoyaltyPointsApi.RequestModels;
using LoyaltyPointsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyPointsApi.Controllers
{
    [ApiController]
    [Route("api/thresholds")]
    public class ThresholdController(IThresholdService thresholdService,
    ILogger<ThresholdController> logger) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> AddThreshold([FromBody] ThresholdRequestModel thresholdRequestModel)
        {
            logger.LogInformation("Add Threshold request: {threshold} for restaurant: {restaurantId}", thresholdRequestModel.ThresholdName, thresholdRequestModel.RestaurantId);
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
            logger.LogInformation("Get Thresholds request for restaurant: {restaurantId}", restaurantId);
            try
            {
                var result = await thresholdService.GetRestaurantThresholds(restaurantId);

                if (result == null) return NotFound(new { success = false, message = "Threshold not found" });

                var responseData = result.Select(t => new
                {
                    thresholdId = t.ThresholdId,
                    restaurantId = t.RestaurantId,
                    thresholdName = t.ThresholdName,
                    minimumPoints = t.MinimumPoints,
                    promotions = t.Promotions?.Select(promo => new
                    {
                        restaurantId = promo.RestaurantId,
                        promoCode = promo.PromoCode,
                        thresholdId = promo.ThresholdId
                    }).ToList()
                }).ToList();

                return Ok(new
                {
                    success = true,
                    message = "Thresholds found",
                    data = responseData
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("restaurants/{restaurantId}")]
        public async Task<ActionResult> GetRestaurantThreshold([FromRoute] int restaurantId,
            [FromQuery] int thresholdId)
        {
            logger.LogInformation("Get Threshold request: {threshold} for restaurant: {restaurantId}", thresholdId, restaurantId);
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
        [Route("restaurants/{restaurantId}/id/{thresholdId}")]
        public async Task<ActionResult> UpdateThreshold([FromBody] ThresholdRequestModel thresholdRequestModel,
            [FromRoute] int restaurantId, [FromRoute] int thresholdId)
        {
            logger.LogInformation("Update Threshold request: {threshold} for restaurant: {restaurantId}", thresholdId, restaurantId);
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

        [HttpDelete]
        [Route("{thresholdId}")]
        public async Task<ActionResult> DeleteThreshold([FromRoute] int thresholdId)
        {
            logger.LogInformation("Delete Threshold request: {thresholdId}", thresholdId);
            try
            {
                await thresholdService.DeleteThreshold(thresholdId);
                return Ok(new { success = true, message = "Threshold deleted", });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}