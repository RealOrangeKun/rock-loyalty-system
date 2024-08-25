using LoyaltyApi.Models;
using LoyaltyApi.Repositories;
using LoyaltyApi.RequestModels;

namespace LoyaltyApi.Services
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        public async Task<User?> CreateUserAsync(RegisterRequestBody registerRequestBody)
        {
            User user = new()
            {
                Name = registerRequestBody.Name,
                Email = registerRequestBody.Email,
                PhoneNumber = registerRequestBody.PhoneNumber,
                Password = registerRequestBody.Password,
                RestaurantId = registerRequestBody.RestaurantId
            };
            return await userRepository.CreateUserAsync(user);
        }

        public async Task<User?> GetUserByEmailAsync(string email, int restaurantId)
        {
            User user = new()
            {
                Email = email,
                RestaurantId = restaurantId
            };
            return await userRepository.GetUserAsync(user);
        }

        public async Task<User?> GetUserByPhonenumberAsync(string phoneNumber, int restaurantId)
        {
            User user = new()
            {
                PhoneNumber = phoneNumber,
                RestaurantId = restaurantId
            };
            return await userRepository.GetUserAsync(user);
        }
    }
}