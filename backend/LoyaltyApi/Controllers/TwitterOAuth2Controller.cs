using LoyaltyApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApi.Controllers
{
    [ApiController]
    [Route("api/oauth2/")]
    public class TwitterOAuth2Controller(OAuth2Service oauth2Service) : ControllerBase
    {
        [HttpGet("signin-twitter")]
        public ActionResult SignInWithTwitter([FromQuery] string restaurantID)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("TwitterCallback", new { restaurantID }),
                Items = { { "LoginProvider", TwitterDefaults.AuthenticationScheme }, { "resId", restaurantID } }
            };
            return Challenge(properties, TwitterDefaults.AuthenticationScheme);
        }

        [HttpGet("signin-twitter/callback")]
        public async Task<IActionResult> TwitterCallback()
        {
            return await oauth2Service.HandleCallbackAsync(HttpContext, TwitterDefaults.AuthenticationScheme);
        }
    }
}