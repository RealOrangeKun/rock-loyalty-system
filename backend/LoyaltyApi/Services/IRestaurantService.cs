using LoyaltyApi.Models;

namespace LoyaltyApi.Services
{
    public interface IRestaurantService
    {
         Task<double> GetCreditPointBuyingRate(int restaurantId);
         Task<double> GetCreditPointSellingRate(int restaurantId);
         Task<double> GetLoyaltyPointBuyingRate(int restaurantId);
         Task<int> GetVoucherLifeTime(int restaurantId);
        Task UpdateCreditBuyingRate(Restaurant restaurant);
        Task UpdateCreditSellingRate(Restaurant restaurant);
        Task UpdateVoucherLifeTime(Restaurant restaurant);

    }
}