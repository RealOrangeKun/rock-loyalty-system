using LoyaltyApi.Config;
using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LoyaltyApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(ITokenService tokenService,
    IOptions<JwtOptions> jwtOptions, IUserService userService,
    IOptions<AdminOptions> adminOptions,
    IPasswordService passwordService,
    ILogger<AuthController> logger) : ControllerBase
    {
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequestBody loginBody)
        {
            logger.LogInformation("Login request for restaurant {RestaurantId}", loginBody.RestaurantId);
            try
            {
                if (loginBody.Email == null && loginBody.PhoneNumber == null) return BadRequest("Email or Phone number is required");
                if (loginBody.Email == adminOptions.Value.Username && loginBody.Password == adminOptions.Value.Password)
                {
                    logger.LogInformation("Admin login request for restaurant {RestaurantId}", loginBody.RestaurantId);
                    string accessTokenAdmin = tokenService.GenerateAccessToken(0, loginBody.RestaurantId, Role.Admin);
                    return Ok(accessTokenAdmin);
                }
                User? user = loginBody.Email != null ? await userService.GetUserByEmailAsync(loginBody.Email, loginBody.RestaurantId) :
                    await userService.GetUserByPhonenumberAsync(loginBody.PhoneNumber ?? throw new ArgumentException("Phone number is required"), loginBody.RestaurantId);
                if (user == null) return Unauthorized();
                Password? password = await passwordService.GetAndValidatePasswordAsync(user.Id, user.RestaurantId, loginBody.Password);
                if (password is null) return Unauthorized();
                if (!password.Confirmed) return Unauthorized();
                string accessToken = tokenService.GenerateAccessToken(user.Id, loginBody.RestaurantId, Role.User);
                string refreshToken = await tokenService.GenerateRefreshTokenAsync(user.Id, loginBody.RestaurantId, Role.User);
                HttpContext.Response.Cookies.Append("refreshToken", refreshToken, jwtOptions.Value.JwtCookieOptions);
                return Ok(accessToken);
            }
            catch (ArgumentException ex)
            {
                logger.LogError(ex, "Login failed for restaurant {RestaurantId}", loginBody.RestaurantId);
                return BadRequest(ex.Message);
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "Login failed for restaurant {RestaurantId}", loginBody.RestaurantId);
                return Unauthorized();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Login failed for restaurant {RestaurantId}", loginBody.RestaurantId);
                return StatusCode(500);
            }
        }
        [HttpGet]
        [Route("confirm-email/{token}")]
        public async Task<ActionResult> ConfirmEmail(string token)
        {
            logger.LogInformation("Confirm email request for token {Token}", token);
            try
            {
                if (token == null) return Unauthorized();
                await passwordService.ConfirmEmail(token);
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Confirm email failed for token {Token}", token);
                return StatusCode(500);
            }
        }
    }
}
