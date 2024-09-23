using LoyaltyPointsApi.Models;
using LoyaltyPointsApi.Repositories;
using LoyaltyPointsApi.RequestModels;

namespace LoyaltyPointsApi.Services
{
    public class PromotionService(IPromotionRepository promotionRepository) : IPromotionService
    {
        public async Task<Promotion?> AddPromotion(PromotionRequestModel promotion)
        {
            Promotion newPromotion = new(){
                RestaurantId = promotion.RestaurantId,
                PromoCode = promotion.PromoCode,
                ThresholdId = promotion.ThresholdId,
            };
            await promotionRepository.AddPromotion(newPromotion);
            return newPromotion;
        }

        public async Task<Promotion?> GetPromotion(string promoCode)
        {
            Promotion promotion = new (){
                PromoCode = promoCode
            };
            var result = await promotionRepository.GetPromotion(promotion);

            return result;
        }

        public async Task<List<Promotion>> GetThresholdPromotions(int thresholdId)
        {
            Promotion promotion = new()
            {
                ThresholdId = thresholdId
            };
            return await promotionRepository.GetThresholdPromotions(promotion);
        }


        public async Task<Promotion?> UpdatePromotion(int id,PromotionRequestModel promotion)
        {
            Promotion promo = new()
            {
                Id = id
            };
            var result  = await promotionRepository.GetPromotion(promo);

            Promotion updatedPromotion = new()
            {
                RestaurantId = promotion.RestaurantId,
                PromoCode = promotion.PromoCode,
                ThresholdId = promotion.ThresholdId
            };
            await promotionRepository.UpdatePromotion(updatedPromotion);
            return updatedPromotion;
        }

    }
}