using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApi.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController(IUserService userService) : ControllerBase
    {
        [HttpPost]
        [Route("")]
        public async Task<ActionResult> CreateUser([FromBody] RegisterRequestBody requestBody)
        {
            try
            {
                if (requestBody.Password == null) throw new ArgumentException("Password cannot be null");
                User? user = await userService.CreateUserAsync(requestBody);
                if (user == null) return StatusCode(500);
                return Ok("User created");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}