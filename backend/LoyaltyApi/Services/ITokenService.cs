using LoyaltyApi.Models;

namespace LoyaltyApi.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(int customerId, int restaurantId, Role role);
        Task<string> GenerateRefreshTokenAsync(int customerId, int restaurantId, Role role);
        bool ValidateRefreshToken(string token);
    }
}