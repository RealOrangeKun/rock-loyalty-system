using LoyaltyApi.Models;

namespace LoyaltyApi.Repositories
{
    public interface IRestaurantRepository
    {
         Task<Restaurant> GetRestaurantInfo(int restaurantId);
         Task UpdateCreditBuyingRate(Restaurant restaurant);
         Task UpdateCreditSellingRate(Restaurant restaurant);

         Task UpdateVoucherLifeTime(Restaurant restaurant);
         Task CreditPointsLifeTime(Restaurant restaurant);
    }
}