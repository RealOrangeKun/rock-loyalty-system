using LoyaltyApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApi.Controllers;

/// <summary>
/// Controller to handle Google OAuth2 authentication.
/// </summary>
[ApiController]
[Route("api/oauth2/")]
public class GoogleOAuth2Controller(OAuth2Service oauth2Service) : ControllerBase
{
    /// <summary>
    /// Initiates the Google sign-in process.
    /// </summary>
    /// <param name="restaurantId">The ID of the restaurant initiating the sign-in.</param>
    /// <returns>
    /// A Challenge result to redirect the user to Google's OAuth2 login page.
    /// </returns>
    /// <response code="302">Redirects to Google's OAuth2 login page.</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/oauth2/signin-google?restaurantId=1
    ///
    /// </remarks>
  [HttpGet("signin-google")]
    public ActionResult SignInWithGoogle([FromQuery] string restaurantId)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action("GoogleCallback", new { restaurantID = restaurantId }),
            Items = { { "LoginProvider", GoogleDefaults.AuthenticationScheme }, { "resId", restaurantId } }
        };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }
    /// <summary>
    /// Handles the callback from Google after the user has authenticated.
    /// </summary>
    /// <returns>
    /// An Ok result if the authentication was successful, or an Unauthorized result if it failed.
    /// </returns>
    /// <response code="200">If the authentication was successful.</response>
    /// <response code="401">If the authentication failed.</response>
    /// <remarks>
    /// This endpoint is called by Google after the user has authenticated.
    /// </remarks>
    [HttpGet("signin-google/callback")]
    public async Task<IActionResult> GoogleCallback()
    {
        return await oauth2Service.HandleCallbackAsync(HttpContext, GoogleDefaults.AuthenticationScheme);
    }
}