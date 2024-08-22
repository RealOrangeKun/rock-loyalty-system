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
            // var user = await userService.ValidateUserAsync(loginBody);
            string accessToken = tokenService.GenerateAccessToken(1, 1);
            string refreshToken = await tokenService.GenerateRefreshTokenAsync(1, 1);
            HttpContext.Response.Cookies.Append("refreshToken", refreshToken, jwtOptions.Value.JwtCookieOptions);
            return Ok(new { accessToken });
        }
    }
}
