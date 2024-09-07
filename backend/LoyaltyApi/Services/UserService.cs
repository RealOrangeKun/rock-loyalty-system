using System.Security.Claims;
using LoyaltyApi.Models;
using LoyaltyApi.Repositories;
using LoyaltyApi.RequestModels;
using Microsoft.AspNetCore.Identity;

namespace LoyaltyApi.Services
{
    public class UserService(IUserRepository userRepository,
        IPasswordService passwordService,
        ILogger<UserService> logger,
        IHttpContextAccessor httpContext) : IUserService
    {
        public async Task<User?> CreateUserAsync(RegisterRequestBody registerRequestBody)
        {
            logger.LogInformation("Creating user {Name}", registerRequestBody.Name);
            User? user = new()
            {
                Name = registerRequestBody.Name,
                Email = registerRequestBody.Email,
                PhoneNumber = registerRequestBody.PhoneNumber,
                RestaurantId = registerRequestBody.RestaurantId
            };
            user = await userRepository.CreateUserAsync(user);
            if (user == null) return null;
            await passwordService.CreatePasswordAsync(user.Id, user.RestaurantId, registerRequestBody.Password);
            return user;
        }

        public async Task<User?> GetUserByEmailAsync(string email, int restaurantId)
        {
            logger.LogInformation("Getting user {Email}", email);
            User user = new()
            {
                Email = email,
                RestaurantId = restaurantId
            };
            return await userRepository.GetUserAsync(user);
        }

        public async Task<User?> GetUserByIdAsync()
        {
            logger.LogInformation("Getting user with id");
            int userId = int.Parse(httpContext.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("userId not found"));
            logger.LogTrace("userId: {userId}", userId);
            int restaurantId = int.Parse(httpContext.HttpContext?.User?.FindFirst("restaurantId")?.Value ?? throw new ArgumentException("restaurantId not found"));
            logger.LogTrace("restaurantId: {restaurantId}", restaurantId);
            User user = new()
            {
                Id = userId,
                RestaurantId = restaurantId
            };
            return await userRepository.GetUserAsync(user);
        }

        public async Task<User?> GetUserByPhonenumberAsync(string phoneNumber, int restaurantId)
        {
            logger.LogInformation("Getting user with phone number {phoneNumber} and restaurantId {restaurantId}", phoneNumber, restaurantId);
            User user = new()
            {
                PhoneNumber = phoneNumber,
                RestaurantId = restaurantId
            };
            return await userRepository.GetUserAsync(user);
        }

        public async Task<User> UpdateUserAsync(UpdateUserRequestModel requestModel)
        {
            logger.LogInformation("Updating user {Name}", requestModel.Name);
            int userId = int.Parse(httpContext.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("userId not found"));
            logger.LogTrace("userId: {userId}", userId);
            int restaurantId = int.Parse(httpContext.HttpContext?.User?.FindFirst("restaurantId")?.Value ?? throw new ArgumentException("restaurantId not found"));
            logger.LogTrace("restaurantId: {restaurantId}", restaurantId);
            User user = new()
            {
                Name = requestModel.Name,
                Email = requestModel.Email,
                PhoneNumber = requestModel.PhoneNumber,
                Id = userId,
                RestaurantId = restaurantId
            };
            return await userRepository.UpdateUserAsync(user);
        }
    }
}