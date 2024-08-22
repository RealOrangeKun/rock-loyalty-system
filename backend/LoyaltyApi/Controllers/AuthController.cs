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
            // TODO: use actual userService
            // var user = await userService.GetAndValidateUserAsync(loginBody);
            // TODO: make random id if testing enviroment
            // if (user == null) return Unauthorized();
            string accessToken = tokenService.GenerateAccessToken(Random.Shared.Next(), loginBody.RestaurantId);
            string refreshToken = await tokenService.GenerateRefreshTokenAsync(Random.Shared.Next(), loginBody.RestaurantId);
            HttpContext.Response.Cookies.Append("refreshToken", refreshToken, jwtOptions.Value.JwtCookieOptions);
            return Ok(new { accessToken });
        }
    }
}
