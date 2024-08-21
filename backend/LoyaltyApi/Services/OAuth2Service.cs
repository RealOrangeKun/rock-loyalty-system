using System.Security.Claims;
using LoyaltyApi.Config;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApi.Services
{
    public class OAuth2Service(ITokenService tokenService, IOptions<JwtOptions> jwtOptions)
    {
        public async Task<IActionResult> HandleCallbackAsync(HttpContext context, string authenticationScheme)
        {
            var result = await context.AuthenticateAsync(authenticationScheme);
            if (!result.Succeeded) return new UnauthorizedResult();

            var claims = result.Principal?.Identities.FirstOrDefault()?.Claims;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            // TODO: Use customer service to check if the customer already exists, if not, add them
            // TODO: Find user with email and restaurant ID. If found, get the customerId and use it below

            var restaurantId = Convert.ToInt32(result.Properties.Items["resId"]);
            var accessToken = tokenService.GenerateAccessToken(1, restaurantId);
            var refreshToken = await tokenService.GenerateRefreshTokenAsync(1, restaurantId);
            context.Response.Cookies.Append("refreshToken", refreshToken, jwtOptions.Value.JwtCookieOptions);
            return new OkObjectResult(new { accessToken });
        }
    }
}