using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Models;

namespace LoyaltyPointsApi.Repositories
{
    public interface IRestaurantRepository
    {
        Task<RestaurantSettings> AddRestaurantSettings(RestaurantSettings restaurant);
        Task<RestaurantSettings?> GetRestaurant(RestaurantSettings restaurant);
        Task<RestaurantSettings> UpdateRestaurant(RestaurantSettings restaurant);
        public Task<List<RestaurantSettings>> GetAllRestaurants();
    }

    
}