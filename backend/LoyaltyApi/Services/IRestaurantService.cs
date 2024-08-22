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
        Task UpdateCreditSellingRate( int restaurantId , double creditPointsSellingRate);
        Task UpdateVoucherLifeTime(int restaurantId,int voucherLifeTime);
        Task UpdateCreditPointsLifeTime(int restaurantId, int creditPointsLifeTime);

    }
}