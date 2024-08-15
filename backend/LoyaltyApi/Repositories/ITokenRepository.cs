namespace LoyaltyApi.Repositories
{
    public interface ITokenRepository
    {
        string GenerateAccessToken(int userId, int restaurantId);

        Task<string> GenerateRefreshTokenAsync(int userId, int restaurantId);

        bool ValidateToken(string token);

        bool ValidateRefreshToken(string token);
    }
}