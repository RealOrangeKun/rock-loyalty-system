using LoyaltyApi.Models;

namespace LoyaltyApi.Repositories
{
    public interface ITokenRepository
    {
        string GenerateAccessToken(Token token);

        Task<string> GenerateRefreshTokenAsync(Token token);

        bool ValidateToken(Token token);

        bool ValidateRefreshToken(Token token);
        bool ValidateConfirmEmailToken(Token token);
        Task<string> GenerateForgotPasswordTokenAsync(Token token);

        Task<string> GenerateConfirmEmailTokenAsync(Token token);
        bool ValidateForgotPasswordTokenAsync(Token token);
    }
}