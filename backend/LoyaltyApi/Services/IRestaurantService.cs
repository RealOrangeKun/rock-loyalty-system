namespace LoyaltyApi.Services
{
    public interface IRestaurantService
    {
         Task<double> GetCreditPointBuyingRate(int restaurantId);
         Task<double> GetCreditPointSellingRate(int restaurantId);
         Task<double> GetLoyaltyPointBuyingRate(int restaurantId);
         Task<int> GetVoucherLifeTime(int restaurantId);
    }
}