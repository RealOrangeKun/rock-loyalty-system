using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LoyaltyApi.Config;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LoyaltyApi.Controllers;

/// <summary>
///  Controller for managing token-related operations.
/// </summary>
[ApiController]
[Route("api/tokens")]
public class TokensController(
    ITokenService tokenService,
    IOptions<JwtOptions> jwtOptions,
    ILogger<TokensController> logger) : ControllerBase
{
    /// <summary>
    /// Refreshes the access and refresh tokens.
    /// </summary>
    /// <remarks>
    /// 
    /// Sample request:
    ///     PUT /api/tokens/refresh-tokens
    ///
    /// Sample response:
    ///     200 OK
    ///     {
    ///         "success": true,
    ///         "message": "Tokens refreshed",
    ///         "data": {
    ///             "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    ///         }
    ///     }
    /// 
    /// Authorization header with JWT Bearer token is required.
    /// </remarks>
    /// <returns> Access Token in the response and Refresh Token is set in a cookie.</returns>
    /// <response code="200">If the tokens are refreshed successfully.</response>
    /// <response code="400">If the refresh token is invalid.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="500">If any other exception occurs.</response>
    [HttpPut]
    [Route("refresh-tokens")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult> RefreshTokens()
    {
        logger.LogInformation("Refresh tokens request for user {UserId}",
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        try
        {
            if (!tokenService.ValidateRefreshToken(HttpContext.Request.Cookies["refreshToken"]))
                return Unauthorized(new { success = false, message = "Invalid refresh token" });
            var (accessToken, refreshToken) = await tokenService.RefreshTokensAsync();
            HttpContext.Response.Cookies.Append("refreshToken", refreshToken, jwtOptions.Value.JwtCookieOptions);
            return Ok(new
            {
                success = true,
                message = "Tokens refreshed",
                data = new { accessToken }
            });
        }
        catch (ArgumentException ex)
        {
            logger.LogError(ex, "Refresh tokens failed for user {UserId}",
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Refresh tokens failed for user {UserId}",
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return StatusCode(StatusCodes.Status500InternalServerError, new
                { success = false, message = ex.Message });
        }
    }
}