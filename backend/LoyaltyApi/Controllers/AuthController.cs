using LoyaltyApi.Models;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(ITokenService tokenService) : ControllerBase
    {
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login([FromBody] AuthBody loginBody)
        {
            // TODO: use actual userService and delete dummy user dictionary
            // var user = await userService.ValidateUserAsync(loginBody);
            var user = new Dictionary<string, int>();
            string accessToken = tokenService.GenerateAccessToken(user["id"], loginBody.RestaurantId);
            string refreshToken = await tokenService.GenerateRefreshTokenAsync(user["id"], loginBody.RestaurantId);
            HttpContext.Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
            return Ok(new { accessToken });
        }
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register([FromBody] AuthBody registernBody)
        {
            // await userService.AddUserAsync(registernBody);
            return Created();
        }
    }
}