using System.Security.Claims;
using LoyaltyApi.Config;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LoyaltyApi.Controllers
{
    [ApiController]
    [Route("api/oauth2/")]
    public class GoogleOAuth2Controller(ITokenService tokenService, IOptions<JwtOptions> jwtOptions) : ControllerBase
    {
        [HttpGet("signin-google")]
        public ActionResult SignInWithGoogle([FromQuery] string restaurantID)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleCallback", new { restaurantID }),
                Items = { { "resId", restaurantID } }
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("signin-google/callback")]
        public async Task<ActionResult> GoogleCallback()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded) return Unauthorized();
            int restaurantId = Convert.ToInt32(result.Properties.Items["resId"]);
            var claims = result.Principal?.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            // TODO: Use customer service to check if the customer already exists, if not, add them
            // TODO: Find user with email and restaurant ID. If found, get the customerId and use it below

            // Generate JWT tokens
            var accessToken = tokenService.GenerateAccessToken(1, restaurantId);
            string refreshToken = await tokenService.GenerateRefreshTokenAsync(1, restaurantId);
            HttpContext.Response.Cookies.Append("refreshToken", refreshToken, jwtOptions.Value.JwtCookieOptions);
            return Ok(new { accessToken });
        }
    }
}