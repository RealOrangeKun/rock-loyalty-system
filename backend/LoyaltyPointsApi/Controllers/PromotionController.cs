using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Models;
using LoyaltyPointsApi.RequestModels;
using LoyaltyPointsApi.Services;
using Microsoft.AspNetCore.Mvc;
namespace LoyaltyPointsApi.Controllers

{
    [ApiController]
    [Route("api/promotions")]
    public class PromotionController(IPromotionService promotionService,
    ILogger<PromotionController> logger) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> AddPromotion([FromBody] AddPromotionRequestModel promotionRequestModel)
        {
            logger.LogInformation("Add Promotion request: {promotion} for restaurant: {restaurantId}", promotionRequestModel.PromoCode, promotionRequestModel.RestaurantId);
            try
            {
                await promotionService.AddPromotion(promotionRequestModel);
                return StatusCode(StatusCodes.Status201Created, new { success = true, message = "Promotion added" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpPut]
        [Route("{promoCode}/restaurants/{restaurantId}")]
        public async Task<ActionResult> UpdatePromotion([FromRoute] string promoCode, [FromRoute] int restaurantId, [FromBody] UpdatePromotionRequestModel promotionRequestModel)
        {
            logger.LogInformation("Update Promotion request: {promotion} for restaurant: {restaurantId}", promoCode, restaurantId);
            try
            {
                await promotionService.UpdatePromotion(promoCode, promotionRequestModel, restaurantId);
                return Ok(new { success = true, message = "Promotion updated" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        [Route("restaurants/{restaurantId}")]
        public async Task<ActionResult> GetRestaurantPromotions([FromRoute] int restaurantId)
        {
            logger.LogInformation("Getting promotions request for restaurant: {restaurantId}", restaurantId);
            try
            {
                var result = await promotionService.GetThresholdPromotions(restaurantId);
                if (result == null) return NotFound(new { success = false, message = "Promotions not found" });
                return Ok(new
                {
                    success = true,
                    message = "Promotions found",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        [Route("{promoCode}")]
        public async Task<ActionResult> GetPromotion([FromRoute] string promoCode)
        {
            logger.LogInformation("Get Promotion Request: {promoCode}", promoCode);
            try
            {
                var result = await promotionService.GetPromotion(promoCode);
                if (result == null) return NotFound(new { success = false, message = "Promotion not found" });
                return Ok(new
                {
                    success = true,
                    message = "Promotion found",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpDelete]
        [Route("{promoCode}")]
        public async Task<ActionResult> DeletePromotion([FromRoute] string promoCode)
        {
            logger.LogInformation("Delete Promotion Request: {promoCode}", promoCode);
            try
            {
                await promotionService.DeletePromotion(promoCode);
                return Ok(new { success = true, message = "Promotion deleted" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}