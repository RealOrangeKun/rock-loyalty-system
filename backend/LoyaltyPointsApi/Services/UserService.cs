using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Models;
using LoyaltyPointsApi.Repositories;
using LoyaltyPointsApi.RequestModels;

namespace LoyaltyPointsApi.Services
{
    public class UserService(IUserRepository userRepository,
    ILogger<UserService> logger) : IUserService
    {

        public async Task<User?> GetUser(UserRequestModel userRequestModel)
        {
            logger.LogInformation("Getting User: {customerId} for restaurant: {restaurantId}", userRequestModel.CustomerId, userRequestModel.RestaurantId);
            User user = new()
            {
                CustomerId = userRequestModel.CustomerId,
                RestaurantId = userRequestModel.RestaurantId
            };

            return await userRepository.GetUser(user);
        }
    }
}