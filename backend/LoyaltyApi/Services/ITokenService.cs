namespace LoyaltyApi.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(int customerId, int restaurantId);
        Task<string> GenerateRefreshTokenAsync(int customerId, int restaurantId);
        bool ValidateRefreshToken(string token);
    }
}