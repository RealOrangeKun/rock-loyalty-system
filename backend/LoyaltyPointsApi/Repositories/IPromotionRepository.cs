using LoyaltyPointsApi.Models;
using X.PagedList;

namespace LoyaltyPointsApi.Repositories
{
    public interface IPromotionRepository
    {
         public Task<Promotion?> AddPromotion(Promotion promotion);
         public Task<Promotion?> UpdatePromotion(Promotion promotion);
        
         public Task<Promotion?> GetPromotion(Promotion promotion);

         public Task<IPagedList<Promotion>> GetThresholdPromotions(Promotion promotion, int pageNumber, int pageSize);

         public Task DeletePromotion(Promotion promotion);
         
         public Task SetPromotionNotified(Promotion promotion);
         
    }
}