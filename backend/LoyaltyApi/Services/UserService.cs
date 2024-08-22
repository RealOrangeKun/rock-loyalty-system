using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyApi.Config;
using LoyaltyApi.Models;
using LoyaltyApi.Repositories;
using LoyaltyApi.Utilities;

namespace LoyaltyApi.Services
{
    public class UserService(IUserRepository userRepository, ApiUtility apiUtility) : IUserService
    {
        public async Task<object> CreateUserAsync(User user)
        {
            return await userRepository.CreateUserAsync(user);
        }

        public async Task<User> GetAndValidateUserAsync(string? phoneNumber,string? email, string? password,int restaurantId)
        {

            User user =  await userRepository.GetUserAsync(email,phoneNumber,restaurantId);
            if(user.Password != password)
            {
                throw new ArgumentException("Invalid password");
            }
            else
            return user;
        }
    }
}