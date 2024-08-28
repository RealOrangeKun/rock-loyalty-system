using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApi.Controllers
{

    [ApiController]
    [Route("api/restaurant")]
    public class RestaurantController(IRestaurantService restaurantService) : Controller
    {
        [HttpPost]
        [Route("")]

        public async Task<ActionResult> CreateRestaurant([FromBody] CreateRestaurantRequestModel createRestaurant)
        {
            try
            {
                CreateRestaurantRequestModel restaurant = new()
                {
                    RestaurantId = createRestaurant.RestaurantId,
                    CreditPointsBuyingRate = createRestaurant.CreditPointsBuyingRate,
                    CreditPointsSellingRate = createRestaurant.CreditPointsSellingRate,
                    LoyaltyPointsBuyingRate = createRestaurant.LoyaltyPointsBuyingRate,
                    LoyaltyPointsSellingRate = createRestaurant.LoyaltyPointsSellingRate,
                    CreditPointsLifeTime = createRestaurant.CreditPointsLifeTime,
                    LoyaltyPointsLifeTime = createRestaurant.LoyaltyPointsLifeTime,
                    VoucherLifeTime = createRestaurant.VoucherLifeTime
                };

                await restaurantService.CreateRestaurant(restaurant);
                return Ok("Restaurant Created");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("{id}")]

        public async Task<ActionResult> GetRestaurant([FromRoute] int id)
        {
            try
            {
                var result = await restaurantService.GetRestaurantInfo(id);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("updaterestuanrt/{id}")]

        public async Task<ActionResult> UpdateRestaurant([FromRoute] int id, [FromBody] RestaurantCreditPointsRequestModel updateRestaurant)
        {
            try
            {
                await restaurantService.UpdateRestaurantInfo(id, updateRestaurant);
                return Ok("Restaurant Updated");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}