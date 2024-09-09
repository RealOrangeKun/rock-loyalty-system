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

namespace LoyaltyApi.Controllers
{
    [ApiController]
    [Route("api/tokens")]
    public class TokensController(ITokenService tokenService,
    IOptions<JwtOptions> jwtOptions,
    ILogger<TokensController> logger) : ControllerBase
    {
        [HttpPut]
        [Route("refresh-tokens")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> RefreshTokens()
        {
            logger.LogInformation("Refresh tokens request for user {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            try
            {
                if (!tokenService.ValidateRefreshToken(HttpContext.Request.Cookies["refreshToken"])) return Unauthorized();
                var (accessToken, refreshToken) = await tokenService.RefreshTokensAsync();
                HttpContext.Response.Cookies.Append("refreshToken", refreshToken, jwtOptions.Value.JwtCookieOptions);
                return Ok(accessToken);
            }
            catch (ArgumentException ex)
            {
                logger.LogError(ex, "Refresh tokens failed for user {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Refresh tokens failed for user {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}