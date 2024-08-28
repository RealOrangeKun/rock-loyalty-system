using LoyaltyApi.Models;
using LoyaltyApi.Repositories;
using LoyaltyApi.RequestModels;
namespace LoyaltyApi.Services
{
    public class RestaurantService(IRestaurantRepository repository) : IRestaurantService
    {
        //Get Methods
        public Task<Restaurant?> GetRestaurantInfo(int restaurantId)
        {
            return repository.GetRestaurantInfo(restaurantId);
        }
        public async Task<double?> GetCreditPointBuyingRate(int restaurantId)
        {
            var result = await repository.GetRestaurantInfo(restaurantId);
            if (result == null) return null;
            return result.CreditPointsBuyingRate;
        }

        public async Task<double?> GetCreditPointSellingRate(int restaurantId)
        {
            var result = await repository.GetRestaurantInfo(restaurantId);
            if (result == null) return null;
            return result.CreditPointsSellingRate;
        }

        public async Task<double?> GetLoyaltyPointBuyingRate(int restaurantId)
        {
            var result = await repository.GetRestaurantInfo(restaurantId);
            if (result == null) return null;
            return result.LoyaltyPointsBuyingRate;
        }
        public async Task<int?> GetVoucherLifeTime(int restaurantId)
        {
            var result = await repository.GetRestaurantInfo(restaurantId);
            if (result == null) return null;

            return result.VoucherLifeTime;
        }


        //Update Methods
        public async Task UpdateRestaurantInfo(int restaurantId, RestaurantCreditPointsRequestModel restaurantRequestModel)
        {
            Restaurant restaurantUpdated = new()
            {
                CreditPointsBuyingRate = restaurantRequestModel.CreditPointsBuyingRate,
                CreditPointsSellingRate = restaurantRequestModel.CreditPointsSellingRate,
                CreditPointsLifeTime = restaurantRequestModel.CreditPointsLifeTime,
                VoucherLifeTime = restaurantRequestModel.VoucherLifeTime,
                RestaurantId = restaurantId,

            };

            await repository.UpdateRestaurant(restaurantId, restaurantUpdated);
        }

        //Create Methods
        public async Task CreateRestaurant(CreateRestaurantRequestModel createRestaurant)
        {
            Restaurant restaurant = new()
            {
                RestaurantId = createRestaurant.RestaurantId,
                CreditPointsBuyingRate = createRestaurant.CreditPointsBuyingRate,
                CreditPointsSellingRate = createRestaurant.CreditPointsSellingRate,
                LoyaltyPointsBuyingRate = createRestaurant.LoyaltyPointsBuyingRate,
                LoyaltyPointsSellingRate = createRestaurant.LoyaltyPointsSellingRate,
                CreditPointsLifeTime = createRestaurant.CreditPointsLifeTime,
                LoyaltyPointsLifeTime = createRestaurant.LoyaltyPointsLifeTime,
                VoucherLifeTime = createRestaurant.VoucherLifeTime
            };

            await repository.CreateRestaurant(restaurant);
        }

    }
}