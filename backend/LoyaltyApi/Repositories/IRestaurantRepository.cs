using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;

namespace LoyaltyApi.Repositories
{
    public interface IRestaurantRepository
    {
         Task<Restaurant?> GetRestaurantInfo(int restaurantId);

         //Update Methods
         Task UpdateRestaurant(int restaurantId, Restaurant resturantRequest);


         //Create Method 
         Task CreateRestaurant(Restaurant restaurant);
    }
}