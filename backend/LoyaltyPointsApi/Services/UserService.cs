using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Models;
using LoyaltyPointsApi.Repositories;
using LoyaltyPointsApi.RequestModels;

namespace LoyaltyPointsApi.Services
{
    public class UserService(UserRepository userRepository) : IUserService
    {
        public async Task AddUser(AddUserRequestModel userRequestModel)
        {
            User user = new(){
                CustomerId = userRequestModel.CustomerId,
                RestaurantId = userRequestModel.RestaurantId,
                LoyaltyPoints = userRequestModel.LoyaltyPoints,
                Rank = userRequestModel.Rank,
            };
            await userRepository.AddUser(user);
        }

        public async Task<User?> GetUser(UserRequestModel userRequestModel)
        {
            User user = new(){
                CustomerId = userRequestModel.CustomerId,
                RestaurantId = userRequestModel.RestaurantId
            };

            return await userRepository.GetUser(user) ;
        }

        public async Task<User?> UpdateUser(UserRequestModel userRequestModel)
        {
            User user = new(){
                CustomerId = userRequestModel.CustomerId,
                RestaurantId = userRequestModel.RestaurantId,
            };
            return await userRepository.UpdateUser(user);
        }
    }
}