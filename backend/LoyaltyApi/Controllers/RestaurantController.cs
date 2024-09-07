using LoyaltyApi.RequestModels;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApi.Controllers
{

    [ApiController]
    [Route("api/admin/restaurants")]
    [Authorize(Roles = "Admin")]
    public class RestaurantController(IRestaurantService restaurantService,
    ILogger<RestaurantController> logger) : Controller
    {
        [HttpPost]
        [Route("")]

        public async Task<ActionResult> CreateRestaurant([FromBody] CreateRestaurantRequestModel createRestaurant)
        {
            logger.LogInformation("Create restaurant request for restaurant with id {id}", createRestaurant.RestaurantId);
            try
            {
                await restaurantService.CreateRestaurant(createRestaurant);
                return StatusCode(StatusCodes.Status201Created, "Restaurant created");
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
            logger.LogInformation("Get restaurant request for restaurant with id {id}", id);
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
        [Route("{id}")]
        public async Task<ActionResult> UpdateRestaurant([FromRoute] int id, [FromBody] RestaurantCreditPointsRequestModel updateRestaurant)
        {
            logger.LogInformation("Update restaurant request for restaurant with id {id}", id);
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