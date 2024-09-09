using LoyaltyApi.Config;
using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LoyaltyApi.Controllers;

/// <summary>
/// Controller for handling authentication-related operations.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController(
    ITokenService tokenService,
    IOptions<JwtOptions> jwtOptions,
    IUserService userService,
    IOptions<AdminOptions> adminOptions,
    IPasswordService passwordService,
    ILogger<AuthController> logger) : ControllerBase
{
    /// <summary>
    /// Authenticates a user and generates a JWT token.
    /// </summary>
    /// <param name="loginBody">The login request body containing email, phone number, password, and restaurant ID.</param>
    /// <returns>
    /// An Ok result containing the generated JWT token, or a BadRequest result if the email or phone number is not provided,
    /// or an Unauthorized result if the credentials are invalid, or an InternalServerError result if any other exception occurs.
    /// </returns>
    /// <response code="200">Returns the generated JWT token.</response>
    /// <response code="400">If the email or phone number is not provided.</response>
    /// <response code="401">If the credentials are invalid.</response>
    /// <response code="500">If any other exception occurs.</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/auth/login
    ///     {
    ///        "email": "user@example.com",
    ///        "phoneNumber": "1234567890",
    ///        "password": "password123",
    ///        "restaurantId": 1
    ///     }
    ///
    /// Sample response:
    ///
    ///     {
    ///        "success": true,
    ///        "message": "Login successful",
    ///        "data": {
    ///            "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    ///        }
    ///     }
    /// </remarks>
    [HttpPost]
    [Route("login")]
    public async Task<ActionResult> Login([FromBody] LoginRequestBody loginBody)
    {
        logger.LogInformation("Login request for restaurant {RestaurantId}", loginBody.RestaurantId);
        try
        {
            if (loginBody.Email == null && loginBody.PhoneNumber == null)
                return BadRequest(new { success = false, message = "Email or Phone number is required" });

            if (loginBody.Email == adminOptions.Value.Username && loginBody.Password == adminOptions.Value.Password)
            {
                logger.LogInformation("Admin login request for restaurant {RestaurantId}", loginBody.RestaurantId);
                string accessTokenAdmin = tokenService.GenerateAccessToken(0, loginBody.RestaurantId, Role.Admin);
                return Ok(new
                {
                    success = true,
                    message = "Admin login successful",
                    data = new { accessToken = accessTokenAdmin }
                });
            }


            User? user = loginBody.Email != null
                ? await userService.GetUserByEmailAsync(loginBody.Email, loginBody.RestaurantId)
                : await userService.GetUserByPhonenumberAsync(
                    loginBody.PhoneNumber ?? throw new ArgumentException("Phone number is required"),
                    loginBody.RestaurantId);
            if (user == null)
                return Unauthorized(new { success = false, message = "Invalid credentials." });

            Password? password =
                await passwordService.GetAndValidatePasswordAsync(user.Id, user.RestaurantId, loginBody.Password);
            if (password is null || !password.Confirmed)
                return Unauthorized(new { success = false, message = "Invalid credentials." });

            string accessToken = tokenService.GenerateAccessToken(user.Id, loginBody.RestaurantId, Role.User);
            string refreshToken =
                await tokenService.GenerateRefreshTokenAsync(user.Id, loginBody.RestaurantId, Role.User);
            HttpContext.Response.Cookies.Append("refreshToken", refreshToken, jwtOptions.Value.JwtCookieOptions);
            return Ok(new
            {
                success = true,
                message = "Login successful",
                data = new { accessToken }
            });
        }
        catch (ArgumentException ex)
        {
            logger.LogError(ex, "Login failed for restaurant {RestaurantId}", loginBody.RestaurantId);
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Login failed for restaurant {RestaurantId}", loginBody.RestaurantId);
            return Unauthorized(new { success = false, message = "Invalid credentials." });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Login failed for restaurant {RestaurantId}", loginBody.RestaurantId);
            return StatusCode(500, new { success = false, message = "Internal server error." });
        }
    }

    /// <summary>
    /// Confirms a user's email based on the provided token.
    /// </summary>
    /// <param name="token">The token used to confirm the email.</param>
    /// <returns>
    /// An Ok result if the email is confirmed successfully, or an Unauthorized result if the token is not provided,
    /// or an InternalServerError result if any other exception occurs.
    /// </returns>
    /// <response code="200">If the email is confirmed successfully.</response>
    /// <response code="401">If the token is not provided.</response>
    /// <response code="500">If any other exception occurs.</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/auth/confirm-email/{token}
    ///
    /// Sample response:
    ///
    ///     {
    ///         "success": true,
    ///        "message": "Email confirmed successfully."
    ///     }
    /// </remarks>
    [HttpGet]
    [Route("confirm-email/{token}")]
    public async Task<ActionResult> ConfirmEmail(string token)
    {
        logger.LogInformation("Confirm email request for token {Token}", token);
        try
        {
            if (token == null)
                return Unauthorized(new { success = false, message = "Token is required." });

            await passwordService.ConfirmEmail(token);
            return Ok(new
            {
                success = true,
                message = "Email confirmed successfully."
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Confirm email failed for token {Token}", token);
            return StatusCode(500, new { success = false, message = "Internal server error." });
        }
    }
}