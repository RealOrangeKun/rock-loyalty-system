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
    public class AuthController(ITokenService tokenService, IOptions<JwtOptions> jwtOptions, ILogger<AuthController> logger) : ControllerBase
    {
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login([FromBody] AuthBody loginBody)
        {
            // TODO: use actual userService and delete dummy user dictionary
            // var user = await userService.ValidateUserAsync(loginBody);
            string accessToken = tokenService.GenerateAccessToken(1, loginBody.RestaurantId);
            string refreshToken = await tokenService.GenerateRefreshTokenAsync(1, loginBody.RestaurantId);
            HttpContext.Response.Cookies.Append("refreshToken", refreshToken, jwtOptions.Value.JwtCookieOptions);
            return Ok(new { accessToken });
        }
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register([FromBody] AuthBody registernBody)
        {
            // await userService.AddUserAsync(registernBody);
            return Created();
        }
        [HttpPost("signin-facebook")]
        public ActionResult SignInWithFacebook([FromQuery] string restaurantID)
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("FacebookCallback", new { restaurantID }) };
            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }
        [HttpGet("signin-facebook/callback")]
        public async Task<ActionResult> FacebookCallback(string restaurantID)
        {
            var result = await HttpContext.AuthenticateAsync(FacebookDefaults.AuthenticationScheme);
            if (!result.Succeeded) return Unauthorized();
            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            //TODO: use customer service to check if the customer already exists if not then add them
            //TODO: find user with email and res id if found then get the cid and put it in first param
            _ = int.TryParse(restaurantID, out var restaurantId);
            var accessToken = tokenService.GenerateAccessToken(1, restaurantId);
            string refreshToken = await tokenService.GenerateRefreshTokenAsync(1, restaurantId);
            HttpContext.Response.Cookies.Append("refreshToken", refreshToken, jwtOptions.Value.JwtCookieOptions);
            return Ok(new { accessToken });
        }

        [HttpGet("signin-google")]
        public ActionResult SignInWithGoogle([FromQuery] string restaurantID)
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleCallback", new { restaurantID }) };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("signin-google/callback")]
        public async Task<ActionResult> GoogleCallback(string restaurantID)
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded) return Unauthorized();

            var claims = result.Principal?.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            // TODO: Use customer service to check if the customer already exists, if not, add them
            // TODO: Find user with email and restaurant ID. If found, get the customerId and use it below
            _ = int.TryParse(restaurantID, out var restaurantId);

            // Generate JWT tokens
            var accessToken = tokenService.GenerateAccessToken(1, restaurantId);
            string refreshToken = await tokenService.GenerateRefreshTokenAsync(1, restaurantId);
            HttpContext.Response.Cookies.Append("refreshToken", refreshToken, jwtOptions.Value.JwtCookieOptions);
            return Ok(new { accessToken });
        }
    }
}
