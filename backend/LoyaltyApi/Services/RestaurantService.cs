using LoyaltyApi.Models;
using LoyaltyApi.Repositories;
using LoyaltyApi.RequestModels;
namespace LoyaltyApi.Services
{
    public class RestaurantService(IRestaurantRepository repository) : IRestaurantService
    {
         //Get Methods
        public Task<Restaurant> GetRestaurantInfo(int restaurantId)
        {
            return repository.GetRestaurantInfo(restaurantId);
        }
        public async Task<double> GetCreditPointBuyingRate(int restaurantId)
        {
            var result = await repository.GetRestaurantInfo(restaurantId);
            return result.CreditPointsBuyingRate;
        }

        public async Task<double> GetCreditPointSellingRate(int restaurantId)
        {
            var result = await repository.GetRestaurantInfo(restaurantId);
            return result.CreditPointsSellingRate;
        }

        public async Task<double> GetLoyaltyPointBuyingRate(int restaurantId)
        {
            var result = await repository.GetRestaurantInfo(restaurantId);
            return result.LoyaltyPointsBuyingRate;
        }
        public async Task<int> GetVoucherLifeTime(int restaurantId)
        {
            var result = await repository.GetRestaurantInfo(restaurantId);
            return result.VoucherLifeTime;
        }


        //Update Methods
        public async Task UpdateCreditBuyingRate(int restaurantId, double creditPointsBuyingRate)
        {
            Restaurant restaurant = await repository.GetRestaurantInfo(restaurantId);
            restaurant.CreditPointsBuyingRate = creditPointsBuyingRate;

            await repository.UpdateCreditBuyingRate(restaurant);
        }

        public async Task UpdateCreditPointsLifeTime(int restaurantId, int creditPointsLifeTime)
        {
            Restaurant restaurant = await repository.GetRestaurantInfo(restaurantId);
            restaurant.CreditPointsLifeTime = creditPointsLifeTime;
        }


        public async Task UpdateCreditSellingRate(int restaurantId, double creditPointsSellingRate)
        {
            Restaurant restaurant = await repository.GetRestaurantInfo(restaurantId);
            restaurant.CreditPointsBuyingRate = creditPointsSellingRate;

            await repository.UpdateCreditSellingRate(restaurant);
        }

        public async Task UpdateVoucherLifeTime(int restaurantId, int voucherLifeTime)
        {
            Restaurant restaurant = await repository.GetRestaurantInfo(restaurantId);
            restaurant.CreditPointsBuyingRate = voucherLifeTime;

            await repository.UpdateVoucherLifeTime(restaurant);
        }


        //Create Methods
          public async Task CreateRestaurant(CreateRestaurantRequestModel createRestaurant)
        {
            Restaurant restaurant = new(){
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

        internal async Task CreateRestaurant(Restaurant restaurant)
        {
            throw new NotImplementedException();
        }
    }
}