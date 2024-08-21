using LoyaltyApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApi.Controllers
{
    [ApiController]
    [Route("api/oauth2/")]
    public class GoogleOAuth2Controller(OAuth2Service oauth2Service) : ControllerBase
    {
        [HttpGet("signin-google")]
        public ActionResult SignInWithGoogle([FromQuery] string restaurantID)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleCallback", new { restaurantID }),
                Items = { { "LoginProvider", GoogleDefaults.AuthenticationScheme }, { "resId", restaurantID } }
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("signin-google/callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            return await oauth2Service.HandleCallbackAsync(HttpContext, GoogleDefaults.AuthenticationScheme);
        }
    }
}