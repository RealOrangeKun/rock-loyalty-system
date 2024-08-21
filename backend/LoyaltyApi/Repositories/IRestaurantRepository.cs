using LoyaltyApi.Models;

namespace LoyaltyApi.Repositories
{
    public interface IRestaurantRepository
    {
         Task<object> GetCreditPointsInfo(int restaurantId);
         Task UpdateCreditPointsInfo(Restaurant restaurant);
    }
}