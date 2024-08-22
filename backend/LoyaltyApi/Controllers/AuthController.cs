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

            User? user = await userService.GetAndValidateUserAsync(loginBody.Email, loginBody.PhoneNumber, loginBody.Password, loginBody.RestaurantId);
            // TODO: make random id if testing enviroment
            if (user == null) return Unauthorized();
            string accessToken = tokenService.GenerateAccessToken(user.Id, loginBody.RestaurantId);
            string refreshToken = await tokenService.GenerateRefreshTokenAsync(user.Id, loginBody.RestaurantId);
            HttpContext.Response.Cookies.Append("refreshToken", refreshToken, jwtOptions.Value.JwtCookieOptions);
            return Ok(new { accessToken });
        }
    }
}
