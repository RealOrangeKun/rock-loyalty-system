using System.Security.Claims;
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
        ILogger<PasswordService> logger,
        IHttpContextAccessor httpContext) : IPasswordService
    {
        public async Task ConfirmEmail(string token)
        {
            if (!tokenService.ValidateConfirmEmailToken(token)) throw new SecurityTokenMalformedException("Invalid Confirm Email Token");
            Token tokenData = tokenUtility.ReadToken(token);
            Password passwordModel = new()
            {
                CustomerId = tokenData.CustomerId,
                RestaurantId = tokenData.RestaurantId,
            };
            Password? password = await repository.GetPasswordAsync(passwordModel) ?? throw new Exception("Password not found");
            password.Confirmed = true;
            await repository.UpdatePasswordAsync(password);
        }

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

        public async Task<Password> UpdatePasswordAsync(int? customerId, int? restaurantId, string password)
        {
            int customerIdJwt = customerId ?? int.Parse(httpContext.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("customerId not found"));
            int restaurantIdJwt = restaurantId ?? int.Parse(httpContext.HttpContext?.User?.FindFirst("restaurantId")?.Value ?? throw new ArgumentException("restaurantId not found"));
            Password passwordModel = new()
            {
                CustomerId = customerId ?? customerIdJwt,
                RestaurantId = restaurantId ?? restaurantIdJwt,
                Value = password,
                Confirmed = true
            };
            string hashedPassword = passwordHasher.HashPassword(passwordModel, password);
            passwordModel.Value = hashedPassword;
            return await repository.UpdatePasswordAsync(passwordModel);
        }
        private bool VerifyPassword(Password password, string providedPassword)
        {
            return passwordHasher.VerifyHashedPassword(password, password.Value, providedPassword) != PasswordVerificationResult.Failed;
        }
    }
}
