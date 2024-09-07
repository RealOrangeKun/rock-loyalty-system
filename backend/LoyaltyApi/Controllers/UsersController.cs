using System.Security.Claims;
using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;
using LoyaltyApi.Services;
using LoyaltyApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LoyaltyApi.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController(IUserService userService,
    ITokenService tokenService,
    ILogger<UsersController> logger,
    EmailUtility emailUtility) : ControllerBase
    {
        [HttpPost]
        [Route("")]
        public async Task<ActionResult> CreateUser([FromBody] RegisterRequestBody requestBody)
        {
            logger.LogInformation("Create user request for restaurant with id {id}", requestBody.RestaurantId);
            try
            {
                User? existingUser = await userService.GetUserByEmailAsync(requestBody.Email, requestBody.RestaurantId) ??
                    await userService.GetUserByPhonenumberAsync(requestBody.PhoneNumber ?? "0", requestBody.RestaurantId);
                if (existingUser is not null) return BadRequest("User already exists");
                if (requestBody.Password == null) throw new ArgumentException("Password cannot be null");
                User? user = await userService.CreateUserAsync(requestBody);
                var confirmToken = await tokenService.GenerateConfirmEmailTokenAsync(user.Id, user.RestaurantId);
                await emailUtility.SendEmailAsync(user.Email, $"Email Confirmation for Loyalty System", "Welcome to Loyalty System. Please Confirm your email by clicking on the following link: http://localhost:5152/api/auth/confirm-email/" + confirmToken, "Rock Loyalty System");
                if (user == null) return StatusCode(500);
                return Ok("User created");
            }
            catch (ArgumentException ex)
            {
                logger.LogError(ex, "Create user failed for restaurant with id {id}", requestBody.RestaurantId);
                return BadRequest(ex.Message);
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "Create user failed for restaurant with id {id}", requestBody.RestaurantId);
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                logger.LogError("Create user failed for restaurant with id {id}", requestBody.RestaurantId);
                return StatusCode(500);
            }
        }
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "User, Admin")]
        public async Task<ActionResult> GetUserById()
        {
            logger.LogInformation("Get user request for user with id {id}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            try
            {
                User? user = await userService.GetUserByIdAsync();
                if (user == null) return NotFound();
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                logger.LogError(ex, "Get user failed for user with id {id}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        [Route("")]
        [Authorize(Roles = "User, Admin")]
        public async Task<ActionResult> UpdateUser([FromBody] UpdateUserRequestModel requestBody)
        {
            logger.LogInformation("Update user request for user with id {id}", User.FindFirst("Id")?.Value);
            try
            {
                User user = await userService.UpdateUserAsync(requestBody);
                if (user == null) return NotFound();
                return Ok("User updated");
            }
            catch (ArgumentException ex)
            {
                logger.LogError(ex, "Update user failed for user with id {id}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Update user failed for user with id {id}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return BadRequest(ex.Message);
            }
        }
    }
}