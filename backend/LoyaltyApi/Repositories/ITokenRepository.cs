using LoyaltyApi.Models;

namespace LoyaltyApi.Repositories
{
    public interface ITokenRepository
    {
        string GenerateAccessToken(Token token);

        Task<string> GenerateRefreshTokenAsync(Token token);

        bool ValidateToken(Token token);

        bool ValidateRefreshToken(Token token);
        Task<string> GenerateForgotPasswordTokenAsync(Token token);
    }
}