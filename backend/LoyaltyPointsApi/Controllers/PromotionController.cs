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
    public class PromotionController(IPromotionService promotionService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> AddPromotion(PromotionRequestModel promotionRequestModel)
        {
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
        [Route("{promoCode}")]
        public async Task<ActionResult> UpdatePromotion([FromRoute] string promoCode,[FromBody] PromotionRequestModel promotionRequestModel)
        {
            try
            {
                await promotionService.UpdatePromotion(promoCode,promotionRequestModel);
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
    }
}