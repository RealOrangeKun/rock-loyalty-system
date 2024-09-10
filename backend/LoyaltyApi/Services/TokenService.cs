using System.Security.Claims;
using LoyaltyApi.Models;
using LoyaltyApi.Repositories;

namespace LoyaltyApi.Services
{
    public class TokenService(ITokenRepository repository,
    ILogger<TokenService> logger) : ITokenService
    {
        public string GenerateAccessToken(int customerId, int restaurantId, Role role)
        {
            logger.LogInformation("Generating access token for customer {customerId} and restaurant {restaurantId}", customerId, restaurantId);
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
            logger.LogInformation("Generating refresh token for customer {customerId} and restaurant {restaurantId}", customerId, restaurantId);
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
            logger.LogInformation("Validating refresh token for token {tokenValue}", tokenValue);
            Token token = new()
            {
                TokenType = TokenType.RefreshToken,
                TokenValue = tokenValue
            };
            return repository.ValidateRefreshToken(token);
        }

        public async Task<(string accessTokenValue, string refreshTokenValue)> RefreshTokensAsync(int customerId, int restaurantId)
        {
            logger.LogInformation("Refreshing tokens for customer {customerId} and restaurant {restaurantId}", customerId, restaurantId);
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
            logger.LogInformation("Generating forgot password token for customer {customerId} and restaurant {restaurantId}", customerId, restaurantId);
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
            logger.LogInformation("Generating confirm email token for customer {customerId} and restaurant {restaurantId}", customerId, restaurantId);
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
            logger.LogInformation("Validating confirm email token for token {token}", token);
            Token tokenModel = new()
            {
                TokenValue = token,
                TokenType = TokenType.ConfirmEmail
            };
            return repository.ValidateConfirmEmailToken(tokenModel);
        }

        public bool ValidateForgotPasswordToken(string token)
        {
            logger.LogInformation("Validating forgot password token for token {token}", token);
            Token tokenModel = new()
            {
                TokenValue = token,
                TokenType = TokenType.ForgotPasswordToken
            };
            return repository.ValidateForgotPasswordTokenAsync(tokenModel);
        }
    }
}