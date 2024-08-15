using LoyaltyApi.Repositories;

namespace LoyaltyApi.Services
{
    public class TokenService(ITokenRepository repository) : ITokenService
    {
        public string GenerateAccessToken(int customerId, int restaurantId)
        {
            return repository.GenerateAccessToken(customerId, restaurantId);
        }

        public async Task<string> GenerateRefreshTokenAsync(int customerId, int restaurantId)
        {
            return await repository.GenerateRefreshTokenAsync(customerId, restaurantId);
        }

        public bool ValidateRefreshToken(string token)
        {
            return repository.ValidateRefreshToken(token);
        }
    }
}