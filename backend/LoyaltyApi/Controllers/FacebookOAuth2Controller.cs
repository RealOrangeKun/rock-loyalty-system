using LoyaltyApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApi.Controllers;

/// <summary>
/// Controller to handle Facebook OAuth2 authentication.
/// </summary>
[ApiController]
[Route("api/oauth2/")]
public class FacebookOAuth2Controller(OAuth2Service oauth2Service) : ControllerBase
{
    /// <summary>
    /// Initiates the Facebook sign-in process.
    /// </summary>
    /// <param name="restaurantId">The ID of the restaurant initiating the sign-in.</param>
    /// <returns>
    /// A Challenge result to redirect the user to Facebook's OAuth2 login page.
    /// </returns>
    /// <response code="302">Redirects to Facebook's OAuth2 login page.</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/oauth2/signin-facebook?restaurantId=1
    ///
    /// </remarks>
    [HttpGet("signin-facebook")]
    public ActionResult SignInWithFacebook([FromQuery] string restaurantId)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action("FacebookCallback", new { restaurantID = restaurantId }),
            Items = { { "LoginProvider", FacebookDefaults.AuthenticationScheme }, { "resId", restaurantId } }
        };
        return Challenge(properties, FacebookDefaults.AuthenticationScheme);
    }
    /// <summary>
    /// Handles the callback from Facebook after the user has authenticated.
    /// </summary>
    /// <returns>
    /// An Ok result if the authentication was successful, or an Unauthorized result if it failed.
    /// </returns>
    /// <response code="200">If the authentication was successful.</response>
    /// <response code="401">If the authentication failed.</response>
    /// <remarks>
    /// This endpoint is called by Facebook after the user has authenticated.
    /// </remarks>
    [HttpGet("signin-facebook/callback")]
    public async Task<IActionResult> FacebookCallback()
    {
        return await oauth2Service.HandleCallbackAsync(HttpContext, FacebookDefaults.AuthenticationScheme);
    }
}