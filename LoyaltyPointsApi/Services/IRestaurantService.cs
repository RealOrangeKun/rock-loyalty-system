using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Models;
using LoyaltyPointsApi.RequestModels;

namespace LoyaltyPointsApi.Services
{
    public interface IRestaurantService
    {
        Task <RestaurantSettings?> GetRestaurant(int ResturantId);
        Task UpdateRestaurant(int restaurantId , UpdateRestaurantRequestModel updateRestaurantRequestModel);

        Task AddRestaurantSettings(RestaurantRequestModel restaurantRequestModel);
    }
}