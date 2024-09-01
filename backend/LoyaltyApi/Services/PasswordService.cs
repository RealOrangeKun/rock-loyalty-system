using LoyaltyApi.Models;
using LoyaltyApi.Repositories;
using Microsoft.AspNetCore.Identity;

namespace LoyaltyApi.Services
{
    public class PasswordService(IPasswordRepository repository,
        IPasswordHasher<Password> passwordHasher,
        ILogger<PasswordService> logger) : IPasswordService
    {
        public async Task<Password> CreatePasswordAsync(int customerId, int restaurantId, string password)
        {
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

        public async Task<Password> UpdatePasswordAsync(int customerId, int restaurantId, string password)
        {
            Password passwordModel = new()
            {
                CustomerId = customerId,
                RestaurantId = restaurantId,
                Value = password
            };
            return await repository.UpdatePasswordAsync(passwordModel);
        }
        private bool VerifyPassword(Password password, string providedPassword)
        {
            return passwordHasher.VerifyHashedPassword(password, password.Value, providedPassword) != PasswordVerificationResult.Failed;

        }
    }
}
