using LoyaltyApi.Config;
using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LoyaltyApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(ITokenService tokenService,
    IOptions<JwtOptions> jwtOptions, IUserService userService,
    IOptions<AdminOptions> adminOptions,
    IPasswordService passwordService) : ControllerBase
    {
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequestBody loginBody)
        {
            try
            {
                if (loginBody.Email == null && loginBody.PhoneNumber == null) return BadRequest("Email or Phone number is required");
                if (loginBody.Email == adminOptions.Value.Username && loginBody.Password == adminOptions.Value.Password)
                {
                    string accessTokenAdmin = tokenService.GenerateAccessToken(0, 0, Role.Admin);
                    return Ok(accessTokenAdmin);
                }
                User? user = loginBody.Email != null ? await userService.GetUserByEmailAsync(loginBody.Email, loginBody.RestaurantId) :
                    await userService.GetUserByPhonenumberAsync(loginBody.PhoneNumber ?? throw new ArgumentException("Phone number is required"), loginBody.RestaurantId);
                if (user == null) return Unauthorized();
                Password? password = await passwordService.GetAndValidatePasswordAsync(user.Id, user.RestaurantId, loginBody.Password);
                if (password is null) return Unauthorized();
                string accessToken = tokenService.GenerateAccessToken(user.Id, loginBody.RestaurantId, Role.User);
                string refreshToken = await tokenService.GenerateRefreshTokenAsync(user.Id, loginBody.RestaurantId, Role.User);
                HttpContext.Response.Cookies.Append("refreshToken", refreshToken, jwtOptions.Value.JwtCookieOptions);
                return Ok(accessToken);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (HttpRequestException)
            {
                return Unauthorized();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

    }
}
