using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;

namespace LoyaltyApi.Repositories
{
    public interface IRestaurantRepository
    {
        Task<Restaurant?> GetRestaurantInfo(int restaurantId);

        //Update Methods
        Task UpdateRestaurant(Restaurant restaurant);


        //Create Method 
        Task CreateRestaurant(Restaurant restaurant);
    }
}