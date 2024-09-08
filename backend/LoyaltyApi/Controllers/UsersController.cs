using System.Security.Claims;
using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;
using LoyaltyApi.Services;
using LoyaltyApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApi.Controllers;

/// <summary>
/// Controller for managing user-related operations.
/// </summary>
[ApiController]
[Route("api/users")]
public class UsersController(
    IUserService userService,
    ITokenService tokenService,
    ILogger<UsersController> logger,
    EmailUtility emailUtility) : ControllerBase
{
    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="requestBody">The request body containing user registration details.</param>
    /// <returns>ActionResult indicating the result of the operation.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/users
    ///     {
    ///         "name": "John Doe",
    ///         "email": "user@example.com",
    ///         "password": "password123",
    ///         "phoneNumber": "1234567890",
    ///         "restaurantId": "1"
    ///     }
    /// 
    /// Sample response:
    ///
    ///     {
    ///         "success": true,
    ///         "message": "User found",
    ///         "data": {
    ///             "user": {
    ///                  "id": "1",
    ///                  "email": "user@example.com",
    ///                  "phoneNumber": "1234567890",
    ///                  "restaurantId": "1",
    ///                  "name": "John Doe",
    ///              }
    ///         }
    ///     }
    ///
    /// Authorization header with JWT Bearer token is required.
    /// </remarks>
    /// <response code="200">If the user is created successfully.</response>
    /// <response code="400">If the request body is invalid.</response>
    /// <response code="500">If any other exception occurs.</response>
    [HttpPost]
    [Route("")]
    public async Task<ActionResult> CreateUser([FromBody] RegisterRequestBody requestBody)
    {
        logger.LogInformation("Create user request for restaurant with id {id}", requestBody.RestaurantId);
        try
        {
            User? existingUser =
                await userService.GetUserByEmailAsync(requestBody.Email, requestBody.RestaurantId) ??
                await userService.GetUserByPhonenumberAsync(requestBody.PhoneNumber ?? "0",
                    requestBody.RestaurantId);
            if (existingUser is not null)
                return BadRequest(new { success = false, message = "User already exists" });
            if (requestBody.Password == null) throw new ArgumentException("Password cannot be null");
            User? user = await userService.CreateUserAsync(requestBody);
            var confirmToken = await tokenService.GenerateConfirmEmailTokenAsync(user.Id, user.RestaurantId);
            await emailUtility.SendEmailAsync(user.Email, $"Email Confirmation for Loyalty System",
                "Welcome to Loyalty System. Please Confirm your email by clicking on the following link: http://localhost:5152/api/auth/confirm-email/" +
                confirmToken, "Rock Loyalty System");
            // TODO: send correct email confirmation link
            if (user == null) return StatusCode(500, new { success = false, message = "User creation failed" });
            return Ok(new { success = true, message = "User created", data = user });
        }
        catch (ArgumentException ex)
        {
            logger.LogError(ex, "Create user failed for restaurant with id {id}", requestBody.RestaurantId);
            return BadRequest(new { success = false, message = $"{ex.Message}" });
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Create user failed for restaurant with id {id}", requestBody.RestaurantId);
            return BadRequest(new { success = false, message = $"{ex.Message}" });
        }
        catch (Exception)
        {
            logger.LogError("Create user failed for restaurant with id {id}", requestBody.RestaurantId);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Retrieves a user by their ID.
    /// </summary>
    /// <returns>ActionResult indicating the result of the operation.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/users
    ///
    /// 
    /// Sample response:
    ///
    ///     {
    ///         "success": true,
    ///         "message": "User found",
    ///         "data": {
    ///             "user": {
    ///                  "id": "1",
    ///                  "email": "user@example.com",
    ///                  "phoneNumber": "1234567890",
    ///                  "restaurantId": "1",
    ///                  "name": "John Doe",
    ///              }
    ///         }
    ///     }
    ///
    /// Authorization header with JWT Bearer token is required.
    /// </remarks>
    /// <response code="200">If the user is found successfully.</response>
    /// <response code="404">If the user is not found.</response>
    /// <response code="500">If any other exception occurs.</response>
    [HttpGet]
    [Route("")]
    [Authorize(Roles = "User, Admin")]
    public async Task<ActionResult> GetUserById()
    {
        logger.LogInformation("Get user request for user with id {id}",
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        try
        {
            User? user = await userService.GetUserByIdAsync();
            if (user == null) return NotFound(new { success = false, message = "User not found" });
            return Ok(new { success = true, message = "User found", data = user });
        }
        catch (ArgumentException ex)
        {
            logger.LogError(ex, "Get user failed for user with id {id}",
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return Unauthorized(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves a user by their ID.
    /// </summary>
    /// <returns>ActionResult indicating the result of the operation.</returns>
    /// <remarks>
    ///
    /// Sample request:
    ///
    ///     PUT /api/users
    ///     {
    ///         "phoneNumber": "9876543210",
    ///         "email": "newemail@example.com",
    ///         "name": "Jane Doe"
    ///     }
    /// 
    /// Sample response:
    ///
    ///     {
    ///         "success": true,
    ///         "message": "User found",
    ///         "data": {
    ///             "user": {
    ///                  "id": "1",
    ///                  "email": "newemail@example.com",
    ///                  "phoneNumber": "9876543210",
    ///                  "restaurantId": "1",
    ///                  "name": "Jane Doe",
    ///              }
    ///         }
    ///     }
    ///
    /// Authorization header with JWT Bearer token is required.
    /// </remarks>
    /// <response code="200">If the user is found successfully.</response>
    /// <response code="404">If the user is not found.</response>
    /// <response code="500">If any other exception occurs.</response>
    [HttpPut]
    [Route("")]
    [Authorize(Roles = "User, Admin")]
    public async Task<ActionResult> UpdateUser([FromBody] UpdateUserRequestModel requestBody)
    {
        logger.LogInformation("Update user request for user with id {id}", User.FindFirst("Id")?.Value);
        try
        {
            User user = await userService.UpdateUserAsync(requestBody);
            if (user == null) return NotFound(new { success = false, message = "User not found" });
            return Ok(new { success = true, message = "User updated", data = new { user } });
        }
        catch (ArgumentException ex)
        {
            logger.LogError(ex, "Update user failed for user with id {id}",
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Update user failed for user with id {id}",
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}