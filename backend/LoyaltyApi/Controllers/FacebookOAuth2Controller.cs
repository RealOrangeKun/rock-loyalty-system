using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApi.Controllers
{
    [ApiController]
    [Route("api/oauth2/")]
    public class FacebookOAuth2Controller(OAuth2Service oauth2Service) : ControllerBase
    {

        [HttpGet("signin-facebook")]
        public ActionResult SignInWithFacebook([FromQuery] string restaurantID)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("FacebookCallback", new { restaurantID }),
                Items = { { "LoginProvider", FacebookDefaults.AuthenticationScheme }, { "resId", restaurantID } }
            };
            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }

        [HttpGet("signin-facebook/callback")]
        public async Task<IActionResult> FacebookCallback()
        {
            return await oauth2Service.HandleCallbackAsync(HttpContext, FacebookDefaults.AuthenticationScheme);
        }
    }
}