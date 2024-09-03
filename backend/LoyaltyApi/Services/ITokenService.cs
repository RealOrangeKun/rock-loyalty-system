using LoyaltyApi.Models;

namespace LoyaltyApi.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(int customerId, int restaurantId, Role role);
        Task<string> GenerateRefreshTokenAsync(int customerId, int restaurantId, Role role);
        Task<string> GenerateForgotPasswordTokenAsync(int customerId, int restaurantId);
        Task<string> GenerateConfirmEmailTokenAsync(int customerId, int restaurantId);
        bool ValidateRefreshToken(string? token);
        bool ValidateConfirmEmailToken(string token);

        Task<(string accessTokenValue, string refreshTokenValue)> RefreshTokensAsync();
    }
}