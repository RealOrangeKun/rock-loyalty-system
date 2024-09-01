using System;
using System.Collections.Generic;
using System.Linq;
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
    public class TokensController(ITokenService tokenService, IOptions<JwtOptions> jwtOptions) : ControllerBase
    {
        [HttpPut]
        [Route("refresh-tokens")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> RefreshTokens()
        {
            try
            {
                if (!tokenService.ValidateRefreshToken(HttpContext.Request.Cookies["refreshToken"])) return Unauthorized();
                var (accessToken, refreshToken) = await tokenService.RefreshTokensAsync();
                HttpContext.Response.Cookies.Append("refreshToken", refreshToken, jwtOptions.Value.JwtCookieOptions);
                return Ok(accessToken);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}