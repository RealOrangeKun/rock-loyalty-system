using System.Security.Claims;
using LoyaltyApi.Models;
using LoyaltyApi.Repositories;

namespace LoyaltyApi.Services
{
    public class TokenService(ITokenRepository repository,
    IHttpContextAccessor httpContext) : ITokenService
    {
        public string GenerateAccessToken(int customerId, int restaurantId, Role role)
        {
            Token token = new()
            {
                CustomerId = customerId,
                RestaurantId = restaurantId,
                TokenType = TokenType.AccessToken,
                Role = role
            };
            return repository.GenerateAccessToken(token);
        }

        public async Task<string> GenerateRefreshTokenAsync(int customerId, int restaurantId, Role role)
        {
            Token token = new()
            {
                CustomerId = customerId,
                RestaurantId = restaurantId,
                TokenType = TokenType.RefreshToken,
                Role = role
            };
            return await repository.GenerateRefreshTokenAsync(token);
        }

        public bool ValidateRefreshToken(string? tokenValue)
        {
            if (tokenValue == null) throw new ArgumentException("Refresh token cannot be null");
            Token token = new()
            {
                TokenType = TokenType.RefreshToken,
                TokenValue = tokenValue
            };
            return repository.ValidateRefreshToken(token);
        }

        public async Task<(string accessTokenValue, string refreshTokenValue)> RefreshTokensAsync()
        {
            int customerId = int.Parse(httpContext.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("customerId not found"));
            int restaurantId = int.Parse(httpContext.HttpContext?.User?.FindFirst("restaurantId")?.Value ?? throw new ArgumentException("restaurantId not found"));
            Token refreshToken = new()
            {
                CustomerId = customerId,
                RestaurantId = restaurantId,
                TokenType = TokenType.RefreshToken,
                Role = Role.User

            };
            Token accessToken = new()
            {
                CustomerId = customerId,
                RestaurantId = restaurantId,
                TokenType = TokenType.AccessToken,
                Role = Role.User
            };
            string refreshTokenValue = await repository.GenerateRefreshTokenAsync(refreshToken);
            string accessTokenValue = repository.GenerateAccessToken(accessToken);
            return (accessTokenValue, refreshTokenValue);
        }

        public async Task<string> GenerateForgotPasswordTokenAsync(int customerId, int restaurantId)
        {
            Token token = new()
            {
                CustomerId = customerId,
                RestaurantId = restaurantId,
                TokenType = TokenType.ForgotPasswordToken
            };
            return await repository.GenerateForgotPasswordTokenAsync(token);
        }

        public async Task<string> GenerateConfirmEmailTokenAsync(int customerId, int restaurantId)
        {
            Token token = new()
            {
                CustomerId = customerId,
                RestaurantId = restaurantId,
                TokenType = TokenType.ConfirmEmail
            };
            return await repository.GenerateConfirmEmailTokenAsync(token);
        }

        public bool ValidateConfirmEmailToken(string token)
        {
            Token tokenModel = new()
            {
                TokenValue = token,
                TokenType = TokenType.ConfirmEmail
            };
            return repository.ValidateConfirmEmailToken(tokenModel);
        }

        public bool ValidateForgotPasswordToken(string token)
        {
            Token tokenModel = new()
            {
                TokenValue = token,
                TokenType = TokenType.ForgotPasswordToken
            };
            return repository.ValidateForgotPasswordTokenAsync(tokenModel);
        }
    }
}