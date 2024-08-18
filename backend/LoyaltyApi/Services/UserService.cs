using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public Task GetAndValidateUserAsync(string phoneNumber, string password)
        {
            throw new NotImplementedException();
        }
    }
}