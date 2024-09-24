using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.RequestModels;
using LoyaltyPointsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyPointsApi.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetUser([FromBody]UserRequestModel userRequestModel)
        {
            try
            {
                var result = await userService.GetUser(userRequestModel);
                if (result == null)  return NotFound(new { success = false, message = "Threshold not found" });

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
    }
}