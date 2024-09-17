using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Models;

namespace LoyaltyPointsApi.Repositories
{
    public interface IRestaurantRepository
    {
        Task AddRestaurantSettings(RestaurantSettings restaurant);
        Task<RestaurantSettings?> GetRestaurant(int ResturantId);
        Task UpdateRestaurant(RestaurantSettings restaurant);
    }

    
}