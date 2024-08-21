using LoyaltyApi.Repositories;
namespace LoyaltyApi.Services
{
    public class RestaurantService(RestaurantRepository repository) : IRestaurantService
    {
        public async Task<double> GetCreditPointBuyingRate(int restaurantId)
        {
            dynamic restaurant = await repository.GetCreditPointsInfo(restaurantId);
            return restaurant.CreditPointsBuyingRate;
        }

        public async Task<double> GetCreditPointSellingRate(int restaurantId)
        {
            dynamic restaurant = await repository.GetCreditPointsInfo(restaurantId);
            return restaurant.CreditPointsSellingRate;
        }

        public async Task<double> GetLoyaltyPointBuyingRate(int restaurantId)
        {
            dynamic restaurant = await repository.GetCreditPointsInfo(restaurantId);
            return restaurant.CreditPointsBuyingRate;
        }

        public async Task<int> GetVoucherLifeTime(int restaurantId)
        {
            dynamic restaurant = await repository.GetCreditPointsInfo(restaurantId);
            return restaurant.VoucherLifeTime;
        }

    }
}