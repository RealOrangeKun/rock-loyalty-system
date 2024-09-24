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
    [Route("api/restaurants")]
    public class RestaurantController(IRestaurantService restaurantService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> AddRestaurantSettings([FromBody] RestaurantRequestModel restaurantRequestModel)
        {
            try
            {
                await restaurantService.AddRestaurantSettings(restaurantRequestModel);
                return StatusCode(StatusCodes.Status201Created, new { success = true, message = "RestaurantSettings added" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpPut]
        [Route("{restaurantId}")]
        public async Task<ActionResult> UpdateRestaurant([FromRoute] int restaurantId, [FromBody] UpdateRestaurantRequestModel updateRestaurantRequestModel)
        {
            try
            {
                var result = await restaurantService.GetRestaurant(restaurantId);
                if (result == null) return NotFound(new { success = false, message = "Restaurant not found" });

                await restaurantService.UpdateRestaurant(restaurantId, updateRestaurantRequestModel);

                return Ok(new { success = true, message = "RestaurantSettings updated" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("{restaurantId}")]
        public async Task<ActionResult> GetRestaurant([FromRoute] int restaurantId)
        {
            try
            {
                var restaurant = await restaurantService.GetRestaurant(restaurantId);
                if (restaurant == null) return NotFound(new { success = false, message = "Restaurant not found" });
                return Ok(new
                {
                    success = true,
                    message = "Restaurant found",
                    data = new
                    {
                        restaurant
                    }
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