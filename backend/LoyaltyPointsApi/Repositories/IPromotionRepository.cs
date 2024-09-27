using LoyaltyPointsApi.Models;

namespace LoyaltyPointsApi.Repositories
{
    public interface IPromotionRepository
    {
         public Task<Promotion?> AddPromotion(Promotion promotion);
         public Task<Promotion?> UpdatePromotion(Promotion promotion);
        
         public Task<Promotion?> GetPromotion(Promotion promotion);

         public Task<List<Promotion>> GetThresholdtPromotions(Promotion promotion);

         public Task DeletePromotion(Promotion promotion);
         
    }
}