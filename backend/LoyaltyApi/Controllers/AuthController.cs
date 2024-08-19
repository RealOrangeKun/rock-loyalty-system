using System.Security.Claims;
using LoyaltyApi.Config;
using LoyaltyApi.Models;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LoyaltyApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(ITokenService tokenService, IOptions<JwtOptions> jwtOptions, IOptions<API> api, IUserService userService) : ControllerBase
    {
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login([FromBody] User user)
        {
            // TODO: use actual userService
            // var user = await userService.ValidateUserAsync(loginBody);
            string accessToken = tokenService.GenerateAccessToken(user.Id, user.RestaurantId);
            string refreshToken = await tokenService.GenerateRefreshTokenAsync(user.Id, user.RestaurantId);
            HttpContext.Response.Cookies.Append("refreshToken", refreshToken, jwtOptions.Value.JwtCookieOptions);
            return Ok(new { accessToken });
        }
    }
}
