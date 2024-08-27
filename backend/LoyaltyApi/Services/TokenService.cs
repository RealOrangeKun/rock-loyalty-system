using LoyaltyApi.Models;
using LoyaltyApi.Repositories;

namespace LoyaltyApi.Services
{
    public class TokenService(ITokenRepository repository) : ITokenService
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

        public bool ValidateRefreshToken(string tokenValue)
        {
            Token token = new()
            {
                TokenType = TokenType.RefreshToken,
                TokenValue = tokenValue
            };
            return repository.ValidateRefreshToken(token);
        }
    }
}