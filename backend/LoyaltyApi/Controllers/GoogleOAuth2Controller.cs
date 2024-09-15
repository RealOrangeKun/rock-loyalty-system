using LoyaltyApi.Config;
using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LoyaltyApi.Controllers;

/// <summary>
/// Controller to handle Google OAuth2 authentication.
/// </summary>
[ApiController]
[Route("api/oauth2/")]
public class GoogleOAuth2Controller(OAuth2Service oauth2Service,
ILogger<GoogleOAuth2Controller> logger,
IUserService userService,
ITokenService tokenService,
IOptions<JwtOptions> jwtOptions) : ControllerBase
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
    public async Task<ActionResult> SignInWithGoogle([FromQuery] int restaurantId, [FromBody] OAuth2Body body)
    {
        try
        {
            var user = await oauth2Service.HandleGoogleSignIn(body.AccessToken);
            var existingUser = await userService.GetUserByEmailAsync(user.Email, restaurantId);
            if (existingUser is null)
            {
                var registerBody = new RegisterRequestBody()
                {
                    Email = user.Email,
                    Name = user.Name,
                    RestaurantId = restaurantId
                };
                existingUser = await userService.CreateUserAsync(registerBody) ?? throw new HttpRequestException("Failed to create user.");
            }
            string accessToken = tokenService.GenerateAccessToken(existingUser.Id, existingUser.RestaurantId, Role.User);
            string refreshToken = await tokenService.GenerateRefreshTokenAsync(existingUser.Id, existingUser.RestaurantId, Role.User);
            HttpContext.Response.Cookies.Append("refreshToken", refreshToken, jwtOptions.Value.JwtCookieOptions);
            return Ok(new
            {
                success = true,
                message = "Login successful",
                data = new
                {
                    accessToken,
                    existingUser
                }
            });
        }
        catch (ArgumentException ex)
        {
            logger.LogError(ex, "Login failed for restaurant {RestaurantId}", restaurantId);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Login failed for restaurant {RestaurantId}", restaurantId);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Login failed for restaurant {RestaurantId}", restaurantId);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

}