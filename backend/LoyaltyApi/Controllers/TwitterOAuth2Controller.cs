using System.Security.Claims;
using LoyaltyApi.Config;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LoyaltyApi.Controllers
{
    [ApiController]
    [Route("api/oauth2/")]
    public class TwitterOAuth2Controller(ITokenService tokenService, IOptions<JwtOptions> jwtOptions) : ControllerBase
    {
        [HttpGet("signin-twitter")]
        public ActionResult SignInWithTwitter([FromQuery] string restaurantID)
        {
            var redirectUri = Url.Action("TwitterCallback", new { restaurantID });
            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUri,
                Items = { { "LoginProvider", TwitterDefaults.AuthenticationScheme }, { "resId", restaurantID } }
            };
            return Challenge(properties, TwitterDefaults.AuthenticationScheme);
        }

        [HttpGet("signin-twitter/callback")]
        public async Task<ActionResult> TwitterCallback()
        {
            var result = await HttpContext.AuthenticateAsync(TwitterDefaults.AuthenticationScheme);
            if (!result.Succeeded) return Unauthorized();
            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            //TODO: use customer service to check if the customer already exists if not then add them
            //TODO: find user with email and res id if found then get the cid and put it in first param
            var restaurantId = Convert.ToInt32(result.Properties.Items["resId"]);
            var accessToken = tokenService.GenerateAccessToken(1, restaurantId);
            string refreshToken = await tokenService.GenerateRefreshTokenAsync(1, restaurantId);
            HttpContext.Response.Cookies.Append("refreshToken", refreshToken, jwtOptions.Value.JwtCookieOptions);
            return Ok(new { accessToken });
        }
    }
}