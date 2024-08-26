using LoyaltyApi.Models;

namespace LoyaltyApi.Repositories
{
    public interface IRestaurantRepository
    {
         Task<Restaurant> GetRestaurantInfo(int restaurantId);

         //Update Methods
         Task UpdateCreditBuyingRate(Restaurant restaurant);
         Task UpdateCreditSellingRate(Restaurant restaurant);

         Task UpdateVoucherLifeTime(Restaurant restaurant);
         Task UpdateCreditPointsLifeTime(Restaurant restaurant);

         //Create Method 
         Task CreateRestaurant(Restaurant restaurant);
    }
}