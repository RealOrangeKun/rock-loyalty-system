using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;
using LoyaltyApi.Services;
using LoyaltyApi.Utilities;
using Microsoft.AspNetCore.Mvc;

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
    ///     {
    ///         "success": true,
    ///         "message": "Forgot password email sent successfully"
    ///     }
    /// </remarks>
    /// <response code="200">If the forgot password email is sent successfully.</response>
    /// <response code="404">If the user is not found.</response>
    /// <response code="500">If any other exception occurs.</response>
    [HttpPost]
    public async Task<ActionResult> SendForgotPasswordEmail([FromBody] ForgotPasswordRequestBody requestBody)
    {
        logger.LogInformation("Forgot password request for user {Email} and restaurant {RestaurantId}",
            requestBody.Email, requestBody.RestaurantId);
        User? user = await userService.GetUserByEmailAsync(requestBody.Email, requestBody.RestaurantId);
        if (user == null) return NotFound(new { success = false, message = "User not found" });
        var forgotPasswordToken = await tokenService.GenerateForgotPasswordTokenAsync(user.Id, user.RestaurantId);
        await emailUtility.SendEmailAsync(user.Email, $"Forgot Password for {user.Name} - {user.Email}",
            $"Your password reset link is http://localhost:5152/api/password/reset/{forgotPasswordToken}",
            "Rock Loyalty System");
        return Ok(new
        {
            success = true, message = "Forgot password email sent successfully"
        });
    }

    /// <summary>
    /// Resets the user's password using the provided token.
    /// </summary>
    /// <param name="token">The token used to reset the password.</param>
    /// <returns>An ActionResult indicating the result of the operation.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/password/reset/{token}
    ///
    /// Sample response:
    ///
    ///     {
    ///         "success": true,
    ///         "message": "Password reset successful"
    ///     }
    /// </remarks>
    /// <response code="200">If the password reset is successful.</response>
    /// <response code="401">If the token is invalid.</response>
    /// <response code="500">If any other exception occurs.</response>
    [HttpGet]
    [Route("reset/{token}")]
    public ActionResult ResetPassword(string token)
    {
        logger.LogInformation("Reset password request for token {Token}", token);
        if (!tokenService.ValidateForgotPasswordToken(token))
            return Unauthorized(new { success = false, message = "Invalid token" });
        return Ok(new
        {
            success = true,
            message = "Password reset successful"
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
    ///     {
    ///         "success": true,
    ///         "message": "Password updated"
    ///     }
    /// </remarks>
    /// <response code="200">If the password is updated successfully.</response>
    /// <response code="500">If any other exception occurs.</response>
    [HttpPut]
    [Route("{token}")]
    public async Task<ActionResult> UpdatePassword(string? token, [FromBody] UpdatePasswordRequestModel requestBody)
    {
        logger.LogInformation("Update password request for customer with {Token}", token);
        if (token == null)
        {
            await passwordService.UpdatePasswordAsync(null, null, requestBody.Password);
            return Ok(new { success = true, message = "Password updated" });
        }

        Token forgotPasswordToken = tokenUtility.ReadToken(token);
        await passwordService.UpdatePasswordAsync(forgotPasswordToken.CustomerId, forgotPasswordToken.RestaurantId,
            requestBody.Password);
        return Ok(new { success = true, message = "Password updated" });
    }
}