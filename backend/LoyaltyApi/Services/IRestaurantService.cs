using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;

namespace LoyaltyApi.Services
{
    public interface IRestaurantService
    {
        //Get Methods
        Task<Restaurant> GetRestaurantInfo(int restaurantId);
        Task<double> GetCreditPointBuyingRate(int restaurantId);
        Task<double> GetCreditPointSellingRate(int restaurantId);
        Task<double> GetLoyaltyPointBuyingRate(int restaurantId);
        Task<int> GetVoucherLifeTime(int restaurantId);

         //Update Methods
        Task UpdateCreditBuyingRate(int restaurantId , double creditPointsBuyingRate);
        Task UpdateCreditSellingRate( int restaurantId , double creditPointsSellingRate);
        Task UpdateVoucherLifeTime(int restaurantId,int voucherLifeTime);
        Task UpdateCreditPointsLifeTime(int restaurantId, int creditPointsLifeTime);

        //Create Methods
        Task CreateRestaurant(CreateRestaurantRequestModel createRestaurant);

    }
}