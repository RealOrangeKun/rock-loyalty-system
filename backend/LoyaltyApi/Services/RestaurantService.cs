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

        public async Task UpdateCreditBuyingRate(Restaurant restaurant)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCreditSellingRate(Restaurant restaurant)
        {
            throw new NotImplementedException();
        }

        public Task UpdateVoucherLifeTime(Restaurant restaurant)
        {
            throw new NotImplementedException();
        }
    }
}