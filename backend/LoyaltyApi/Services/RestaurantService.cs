using LoyaltyApi.Models;
using LoyaltyApi.Repositories;
namespace LoyaltyApi.Services
{
    public class RestaurantService(RestaurantRepository repository) : IRestaurantService
    {
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

        public async Task UpdateCreditBuyingRate(int restaurantId , double creditPointsBuyingRate)
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


        public async Task UpdateCreditSellingRate(int restaurantId , double creditPointsSellingRate)
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
    }
}