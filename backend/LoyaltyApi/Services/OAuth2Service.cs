using System.Security.Claims;
using LoyaltyApi.Config;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using LoyaltyApi.Models;
using LoyaltyApi.Repositories;

namespace LoyaltyApi.Services
{
    public class OAuth2Service(ITokenService tokenService, IOptions<JwtOptions> jwtOptions, IUserService userService, IUserRepository userRepository)
    {
        public async Task<IActionResult> HandleCallbackAsync(HttpContext context, string authenticationScheme)
        {
            var result = await context.AuthenticateAsync(authenticationScheme);
            if (!result.Succeeded) return new UnauthorizedResult();

            var claims = result.Principal?.Identities.FirstOrDefault()?.Claims;
            string? name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? throw new ArgumentException("Name not found");
            string? email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? throw new ArgumentException("Email not found");
            var restaurantId = Convert.ToInt32(result.Properties.Items["resId"]);
            User? user = await userService.GetUserByEmailAsync(email, restaurantId);
            if (user == null)
            {
                user = new()
                {
                    Email = email,
                    Name = name,
                    RestaurantId = restaurantId,
                    PhoneNumber = "123"
                };
                await userRepository.CreateUserAsync(user);
            };
            var accessToken = tokenService.GenerateAccessToken(user.Id, user.RestaurantId, Role.User);
            var refreshToken = await tokenService.GenerateRefreshTokenAsync(user.Id, user.RestaurantId, Role.User);
            context.Response.Cookies.Append("refreshToken", refreshToken, jwtOptions.Value.JwtCookieOptions);
            return new OkObjectResult(accessToken);
        }
    }
}