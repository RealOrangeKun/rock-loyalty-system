using LoyaltyPointsApi.Models;
using LoyaltyPointsApi.RequestModels;

namespace LoyaltyPointsApi.Services
{
    public interface IPromotionService
    {
         public Task<Promotion?> AddPromotion(PromotionRequestModel promotion);
         public Task<Promotion?> GetPromotion(int id );
         public Task<Promotion?> UpdatePromotion(int id ,PromotionRequestModel promotion);

         public Task<List<Promotion>> GetRestaurantPromotions(int restaurantId);
    }
}