using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;
using LoyaltyApi.Services;
using LoyaltyApi.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApi.Controllers
{
    [ApiController]
    [Route("api/password")]
    public class PasswordController(ITokenService tokenService,
    EmailUtility emailUtility,
    IUserService userService,
    IPasswordService passwordService,
    ILogger<PasswordController> logger,
    TokenUtility tokenUtility) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> SendForgotPasswordEmail([FromBody] ForgotPasswordRequestBody requestBody)
        {
            logger.LogInformation("Forgot password request for user {Email} and restaurant {RestaurantId}", requestBody.Email, requestBody.RestaurantId);
            User? user = await userService.GetUserByEmailAsync(requestBody.Email, requestBody.RestaurantId);
            if (user == null) return NotFound();
            var forgotPasswordToken = await tokenService.GenerateForgotPasswordTokenAsync(user.Id, user.RestaurantId);
            await emailUtility.SendEmailAsync(user.Email, $"Forgot Password for {user.Name} - {user.Email}", $"Your password reset link is http://localhost:5152/api/password/reset/{forgotPasswordToken}", "Rock Loyalty System");
            return Ok();
        }
        [HttpGet]
        [Route("reset/{token}")]
        public ActionResult ResetPassword(string token)
        {
            logger.LogInformation("Reset password request for token {Token}", token);
            if (token == null) return Unauthorized();
            if (!tokenService.ValidateForgotPasswordToken(token)) return Unauthorized();
            return Ok();
        }
        [HttpPut]
        [Route("{token}")]
        public async Task<ActionResult> UpdatePassword(string? token, [FromBody] UpdatePasswordRequestModel requestBody)
        {
            logger.LogInformation("Update password request for customer with {Token}", token);
            if (token == null)
            {
                await passwordService.UpdatePasswordAsync(null, null, requestBody.Password);
                return Ok("Password updated");
            };
            Token forgotPasswordToken = tokenUtility.ReadToken(token);
            await passwordService.UpdatePasswordAsync(forgotPasswordToken.CustomerId, forgotPasswordToken.RestaurantId, requestBody.Password);
            return Ok("Password updated");
        }
    }

}