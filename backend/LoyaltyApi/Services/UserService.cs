using LoyaltyApi.Models;
using LoyaltyApi.Repositories;

namespace LoyaltyApi.Services
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        public async Task<object> CreateUserAsync(User user)
        {
            return await userRepository.CreateUserAsync(user);
        }

        public async Task<User?> GetAndValidateUserAsync(string? phoneNumber, string? email, string? password, int restaurantId)
        {

            User? user = await userRepository.GetUserAsync(email, phoneNumber, restaurantId);
            if (user == null) return null;
            if (user.Password != password) throw new ArgumentException("Invalid password");
            return user;
        }
    }
}