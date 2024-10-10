using LoyaltyPointsApi.Models;
using LoyaltyPointsApi.RequestModels;
using X.PagedList;

namespace LoyaltyPointsApi.Services
{
    public interface IPromotionService
    {
        public Task<Promotion?> AddPromotion(AddPromotionRequestModel promotion);
        public Task<Promotion?> GetPromotion(string promoCode);
        public Task<Promotion?> UpdatePromotion(String promoCode, UpdatePromotionRequestModel promotion, int restaurantId);

        public Task<IPagedList<Promotion>> GetThresholdPromotions(int thresholdId, int pageNumber, int pageSize);
        public Task DeletePromotion(String promoCode);
        public Task SetPromotionNotified(string promoCode);
    }
}