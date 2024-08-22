using System.Security.Claims;
using LoyaltyApi.Config;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LoyaltyApi.Models;

namespace LoyaltyApi.Services
{
    public class OAuth2Service(ITokenService tokenService, IOptions<JwtOptions> jwtOptions, IUserService userService)
    {
        public async Task<IActionResult> HandleCallbackAsync(HttpContext context, string authenticationScheme)
        {
            var result = await context.AuthenticateAsync(authenticationScheme);
            if (!result.Succeeded) return new UnauthorizedResult();

            var claims = result.Principal?.Identities.FirstOrDefault()?.Claims;
            string? name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? throw new ArgumentException("Name not found");
            string? email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? throw new ArgumentException("Email not found");
            var restaurantId = Convert.ToInt32(result.Properties.Items["resId"]);
            User? user = await userService.GetAndValidateUserAsync(null, email, null, restaurantId);
            if (user == null)
            {
                user = new()
                {
                    Email = email,
                    Name = name,
                    RestaurantId = restaurantId,
                    PhoneNumber = "123"
                };
                await userService.CreateUserAsync(user);
            };
            var accessToken = tokenService.GenerateAccessToken(1, restaurantId);
            var refreshToken = await tokenService.GenerateRefreshTokenAsync(1, restaurantId);
            context.Response.Cookies.Append("refreshToken", refreshToken, jwtOptions.Value.JwtCookieOptions);
            return new OkObjectResult(new { accessToken });
        }
    }
}