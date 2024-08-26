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

       public async Task<ActionResult> CreateRestaurant([FromBody] CreateRestaurantRequestModel createRestaurant){
            try
            {
                CreateRestaurantRequestModel restaurant = new(){
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

       public async Task<ActionResult> GetRestaurant([FromRoute]int id){
           try
           {
               var result = await restaurantService.GetRestaurantInfo(id);
               return Ok(result);
           }
           catch (Exception ex)
           {
               return BadRequest(ex.Message);
           }
       }

       [HttpPut]
       [Route("creditpointsbuyingrate/{id}")]

       public async Task<ActionResult> UpdateCreditBuyingRate ([FromRoute]int id, [FromBody] double creditPointsBuyingRate){
           try
           {
               await restaurantService.UpdateCreditBuyingRate(id, creditPointsBuyingRate);
               return Ok("Credit buying rate updated");
           }
           catch (Exception ex)
           {
               return BadRequest(ex.Message);
        
     }
       }
        [HttpPut]
        [Route("creditpointssellingrate/{id}")]
       public async Task<ActionResult> UpdateCreditSellingRate ([FromRoute]int id, [FromBody] double creditPointsSellingRate){
           try
           {
               await restaurantService.UpdateCreditSellingRate(id, creditPointsSellingRate);
               return Ok("Credit selling rate updated");
           }    
           catch (Exception ex)
           {
               return BadRequest(ex.Message);
           }
       }
       [HttpPut]
       [Route("voucherlifetime/{id}")]
       public async Task<ActionResult> UpdateVoucherLifeTime ([FromRoute]int id, [FromBody] int voucherLifeTime){ 
           try
           {
               await restaurantService.UpdateVoucherLifeTime(id, voucherLifeTime);
               return Ok("Voucher lifetime updated");
           }
           catch (Exception ex)
           {
               return BadRequest(ex.Message);
           }
       }
}
}