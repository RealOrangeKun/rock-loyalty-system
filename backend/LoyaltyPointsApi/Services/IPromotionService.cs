using LoyaltyPointsApi.Models;
using LoyaltyPointsApi.RequestModels;

namespace LoyaltyPointsApi.Services
{
    public interface IPromotionService
    {
         public Task<Promotion?> AddPromotion(AddPromotionRequestModel promotion);
         public Task<Promotion?> GetPromotion(string promoCode );
         public Task<Promotion?> UpdatePromotion(String promoCode ,UpdatePromotionRequestModel promotion,int restaurantId);

         public Task<List<Promotion>> GetThresholdPromotions(int thresholdId);
         public Task DeletePromotion(String promoCode);
    }
}   