using LoyaltyApi.Models;

namespace LoyaltyApi.Services
{
    public interface IRestaurantService
    {
         Task<double> GetCreditPointBuyingRate(int restaurantId);
         Task<double> GetCreditPointSellingRate(int restaurantId);
         Task<double> GetLoyaltyPointBuyingRate(int restaurantId);
         Task<int> GetVoucherLifeTime(int restaurantId);
        Task UpdateCreditBuyingRate(int restaurantId , double creditPointsBuyingRate);
        Task UpdateCreditSellingRate( int restaurant , double creditPointsSellingRate);
        Task UpdateVoucherLifeTime(int restaurant,int voucherLifeTime);
        Task UpdateCreditPointsLifeTime(int restaurant, int creditPointsLifeTime);

    }
}