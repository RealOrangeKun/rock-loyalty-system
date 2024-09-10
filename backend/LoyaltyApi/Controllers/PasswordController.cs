using System.Security.Claims;
using LoyaltyApi.Config;
using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;
using LoyaltyApi.Services;
using LoyaltyApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LoyaltyApi.Controllers;

/// <summary>
/// Controller responsible for handling password-related operations.
/// </summary>
[ApiController]
[Route("api/password")]
public class PasswordController(
    ITokenService tokenService,
    EmailUtility emailUtility,
    IUserService userService,
    IPasswordService passwordService,
    ILogger<PasswordController> logger,
    IOptions<FrontendOptions> frontendOptions,
    TokenUtility tokenUtility) : ControllerBase
{
    /// <summary>
    /// Sends a forgot password email to the user.
    /// </summary>
    /// <param name="requestBody">The request body containing the user's email and restaurant ID.</param>
    /// <returns>An ActionResult indicating the result of the operation.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/password
    ///     {
    ///         "email": "user@example.com",
    ///         "restaurantId": "12"
    ///     }
    ///
    /// Sample response:
    ///
    ///     200 OK
    ///     {
    ///         "success": true,
    ///         "message": "Forgot password email sent successfully"
    ///     }
    /// </remarks>
    /// <response code="200">If the forgot password email is sent successfully.</response>
    /// <response code="404">If the user is not found.</response>
    /// <response code="500">If any other exception occurs.</response>
    [HttpPost]
    [Route("forgot-password-email")]
    public async Task<ActionResult> SendForgotPasswordEmail([FromBody] ForgotPasswordRequestBody requestBody)
    {
        logger.LogInformation("Forgot password request for user {Email} and restaurant {RestaurantId}",
            requestBody.Email, requestBody.RestaurantId);
        User? user = await userService.GetUserByEmailAsync(requestBody.Email, requestBody.RestaurantId);
        if (user == null) return NotFound(new { success = false, message = "User not found" });
        var forgotPasswordToken = await tokenService.GenerateForgotPasswordTokenAsync(user.Id, user.RestaurantId);
        await emailUtility.SendEmailAsync(user.Email, $"Forgot Password for {user.Name} - {user.Email}",
            $"Your password reset link is {frontendOptions.Value.BaseUrl}/password/reset/{forgotPasswordToken}",
            "Rock Loyalty System");
        return StatusCode(201, new
        {
            success = true,
            message = "Forgot password email sent successfully"
        });
    }

    /// <summary>
    /// Updates the user's password.
    /// </summary>
    /// <param name="token">The token used to validate the password update request.</param>
    /// <param name="requestBody">The request body containing the new password.</param>
    /// <returns>An ActionResult indicating the result of the operation.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /api/password/{token}
    ///     {
    ///         "password": "newpassword123"
    ///     }
    ///
    /// Sample response:
    ///
    ///     200 OK
    ///     {
    ///         "success": true,
    ///         "message": "Password updated"
    ///     }
    /// </remarks>
    /// <response code="200">If the password is updated successfully.</response>
    /// <response code="500">If any other exception occurs.</response>
    [HttpPut]
    [Route("{token}")]
    public async Task<ActionResult> UpdatePasswordWithForgotPasswordToken(string token, [FromBody] UpdatePasswordRequestModel requestBody)
    {
        logger.LogInformation("Update password request for customer with {Token}", token);
        if (!tokenService.ValidateForgotPasswordToken(token)) return Unauthorized(new { success = false, message = "Invalid token" });
        Token forgotPasswordToken = tokenUtility.ReadToken(token);
        await passwordService.UpdatePasswordAsync(forgotPasswordToken.CustomerId, forgotPasswordToken.RestaurantId,
            requestBody.Password);
        return Ok(new { success = true, message = "Password updated" });
    }
    [HttpPut]
    [Route("")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult> UpdatePassword([FromBody] UpdatePasswordRequestModel requestBody)
    {
        logger.LogInformation("Update password request for customer {CustomerId} and restaurant {RestaurantId}",
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value, User.FindFirst("restaurantId")?.Value);
        try
        {
            string userClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException();
            string restaurantClaim = User.FindFirst("restaurantId")?.Value ?? throw new UnauthorizedAccessException();
            _ = int.TryParse(userClaim, out var userId);
            _ = int.TryParse(restaurantClaim, out var restaurantId);
            if (userId == null || restaurantId == null) return Unauthorized(new { success = false, message = "Token parameters could not be read or the token is invalid" });
            await passwordService.UpdatePasswordAsync(userId, restaurantId, requestBody.Password);
            return Ok(new { success = true, message = "Password updated" });
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogError(ex, "Unauthorized access");
            return Unauthorized(new { success = false, message = "Unauthorized access" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Update password failed for customer {CustomerId} and restaurant {RestaurantId}",
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value, User.FindFirst("restaurantId")?.Value);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
}