using LoyaltyApi.Models;

namespace LoyaltyApi.Repositories
{
    public interface IRestaurantRepository
    {
         Task<Restaurant> GetRestaurantInfo(int restaurantId);
         Task UpdateCreditPointsInfo(Restaurant restaurant);
    }
}