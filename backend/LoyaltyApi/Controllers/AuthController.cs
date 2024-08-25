using LoyaltyApi.Config;
using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LoyaltyApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(ITokenService tokenService, IOptions<JwtOptions> jwtOptions, IUserService userService) : ControllerBase
    {
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequestBody loginBody)
        {
            try
            {
                if (loginBody.Email == null && loginBody.PhoneNumber == null) return BadRequest("Email or Phone number is required");
                User? user = loginBody.Email != null ? await userService.GetUserByEmailAsync(loginBody.Email, loginBody.RestaurantId) :
                    await userService.GetUserByPhonenumberAsync(loginBody.PhoneNumber ?? throw new ArgumentException("Phone number is required"), loginBody.RestaurantId);
                // TODO: make random id if testing enviroment
                if (user == null) return Unauthorized();
                string accessToken = tokenService.GenerateAccessToken(user.Id, loginBody.RestaurantId);
                string refreshToken = await tokenService.GenerateRefreshTokenAsync(user.Id, loginBody.RestaurantId);
                HttpContext.Response.Cookies.Append("refreshToken", refreshToken, jwtOptions.Value.JwtCookieOptions);
                return Ok(accessToken);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (HttpRequestException)
            {
                return Unauthorized();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

        }
    }
}
