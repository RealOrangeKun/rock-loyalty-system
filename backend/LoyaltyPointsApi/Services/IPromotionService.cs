using LoyaltyPointsApi.Models;
using LoyaltyPointsApi.RequestModels;

namespace LoyaltyPointsApi.Services
{
    public interface IPromotionService
    {
         public Task<Promotion?> AddPromotion(PromotionRequestModel promotion);
         public Task<Promotion?> GetPromotion(string promoCode );
         public Task<Promotion?> UpdatePromotion(String promoCode ,PromotionRequestModel promotion);

         public Task<List<Promotion>> GetThresholdPromotions(int thresholdId);
    }
}   