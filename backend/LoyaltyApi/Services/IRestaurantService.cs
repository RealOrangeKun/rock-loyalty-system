using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;

namespace LoyaltyApi.Services
{
    public interface IRestaurantService
    {
        //Get Methods
        Task<Restaurant?> GetRestaurantInfo(int restaurantId);
        Task<double?> GetCreditPointBuyingRate(int restaurantId);
        Task<double?> GetCreditPointSellingRate(int restaurantId);
        Task<double?> GetLoyaltyPointBuyingRate(int restaurantId);
        Task<int?> GetVoucherLifeTime(int restaurantId);

         //Update Methods
        Task UpdateRestaurantInfo(int resturantId,RestaurantCreditPointsRequestModel createRestaurantModel);

        //Create Methods
        Task CreateRestaurant(CreateRestaurantRequestModel createRestaurant);

    }
}