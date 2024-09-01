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
                User? existingUser = await userService.GetUserByEmailAsync(requestBody.Email, requestBody.RestaurantId) ??
                    await userService.GetUserByPhonenumberAsync(requestBody.PhoneNumber ?? "0", requestBody.RestaurantId);
                if (existingUser is not null) return BadRequest("User already exists");
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
        [HttpGet]
        [Route("")]
        public async Task<ActionResult> GetUserById()
        {
            try
            {
                User? user = await userService.GetUserByIdAsync();
                if (user == null) return NotFound();
                return Ok(user);
            }
            catch (ArgumentException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        [Route("")]
        public async Task<ActionResult> UpdateUser([FromBody] UpdateUserRequestModel requestBody)
        {
            try
            {
                User user = await userService.UpdateUserAsync(requestBody);
                if (user == null) return NotFound();
                return Ok("User updated");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}