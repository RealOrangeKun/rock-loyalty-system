using System.Security.Claims;
using LoyaltyApi.Config;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LoyaltyApi.Controllers
{
    [ApiController]
    [Route("api/oauth2/")]
    public class FacebookOAuth2Controller(ITokenService tokenService, IOptions<JwtOptions> jwtOptions) : ControllerBase
    {
        [HttpGet("signin-facebook")]
        public ActionResult SignInWithFacebook([FromQuery] string restaurantID)
        {
            var redirectUri = Url.Action("FacebookCallback", new { restaurantID });

            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUri,
                Items = { { "LoginProvider", FacebookDefaults.AuthenticationScheme }, { "resId", restaurantID } }
            };
            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }
        [HttpGet("signin-facebook/callback")]
        public async Task<ActionResult> FacebookCallback()
        {
            var result = await HttpContext.AuthenticateAsync(FacebookDefaults.AuthenticationScheme);
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