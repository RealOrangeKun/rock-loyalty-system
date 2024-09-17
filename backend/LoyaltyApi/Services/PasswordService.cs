using System.Security.Claims;
using LoyaltyApi.Exceptions;
using LoyaltyApi.Models;
using LoyaltyApi.Repositories;
using LoyaltyApi.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace LoyaltyApi.Services
{
    public class PasswordService(IPasswordRepository repository,
        IPasswordHasher<Password> passwordHasher,
        ITokenService tokenService,
        TokenUtility tokenUtility,
        ILogger<PasswordService> logger) : IPasswordService
    {
        public async Task<Password?> GetPasswordByCustomerIdAsync(int customerId, int restaurantId)
        {
            Password password = new()
            {
                CustomerId = customerId,
                RestaurantId = restaurantId
            };
            return await repository.GetPasswordAsync(password);
        }
        public async Task ConfirmEmail(string token)
        {
            logger.LogInformation("Confirming email with token {token}", token);
            if (!tokenService.ValidateConfirmEmailToken(token)) throw new SecurityTokenMalformedException("Invalid Confirm Email Token");
            Token tokenData = tokenUtility.ReadToken(token);
            Password passwordModel = new()
            {
                CustomerId = tokenData.CustomerId,
                RestaurantId = tokenData.RestaurantId,
            };
            Password? password = await repository.GetPasswordAsync(passwordModel) ?? throw new Exception("Password not found");
            if (password.Confirmed) throw new EmailAlreadyConfirmedException("Email already confirmed");
            password.Confirmed = true;
            await repository.UpdatePasswordAsync(password);
        }

        public async Task<Password> CreatePasswordAsync(int customerId, int restaurantId, string password)
        {
            logger.LogInformation("Creating password for customer {customerId} and restaurant {restaurantId}", customerId, restaurantId);
            if (password is null) throw new ArgumentException("Password cannot be null");
            Password passwordModel = new()
            {
                CustomerId = customerId,
                RestaurantId = restaurantId,
            };
            string hashedPassword = passwordHasher.HashPassword(passwordModel, password);
            passwordModel.Value = hashedPassword;
            return await repository.CreatePasswordAsync(passwordModel);
        }


        public async Task<Password?> GetAndValidatePasswordAsync(int customerId, int restaurantId, string inputPassword)
        {
            logger.LogInformation("Getting and validating password for customer {customerId} and restaurant {restaurantId}", customerId, restaurantId);
            Password passwordModel = new()
            {
                CustomerId = customerId,
                RestaurantId = restaurantId
            };
            Password? password = await repository.GetPasswordAsync(passwordModel);
            if (password is null) return null;
            if (!VerifyPassword(password, inputPassword)) return null;
            return password;
        }

        public async Task<Password> UnConfirmEmail(int customerId, int restaurantId)
        {
            logger.LogInformation("Unconfirming email for customer {customerId} and restaurant {restaurantId}", customerId, restaurantId);
            Password passwordModel = new()
            {
                CustomerId = customerId,
                RestaurantId = restaurantId
            };
            return await repository.UpdatePasswordAsync(passwordModel);
        }

        public async Task<Password> UpdatePasswordAsync(int customerId, int restaurantId, string password)
        {
            Password passwordModel = new()
            {
                CustomerId = customerId,
                RestaurantId = restaurantId,
                Value = password,
                Confirmed = true
            };
            _ = await repository.GetPasswordAsync(passwordModel) ?? throw new ArgumentException("Password doesn't exist");
            string hashedPassword = passwordHasher.HashPassword(passwordModel, password);
            logger.LogTrace("hashedPassword: {hashedPassword}", hashedPassword);
            passwordModel.Value = hashedPassword;
            return await repository.UpdatePasswordAsync(passwordModel);
        }
        private bool VerifyPassword(Password password, string providedPassword)
        {
            logger.LogInformation("Verifying password");
            return passwordHasher.VerifyHashedPassword(password, password.Value, providedPassword) != PasswordVerificationResult.Failed;
        }
    }
}
