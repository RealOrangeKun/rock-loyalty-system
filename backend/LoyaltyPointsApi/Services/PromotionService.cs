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
            return await promotionRepository.GetThresholdtPromotions(promotion);
        }


        public async Task<Promotion?> UpdatePromotion(string promoCode,PromotionRequestModel promotion)
        {
            Promotion rxistingPromotion = new(){
                PromoCode = promoCode
            };
            var result  = await promotionRepository.GetPromotion(rxistingPromotion);

            if(result == null)
            {
                throw new Exception("Promotion not found");
            }
            result.RestaurantId = promotion.RestaurantId;
            result.PromoCode = promotion.PromoCode;
            result.ThresholdId = promotion.ThresholdId;
            await promotionRepository.UpdatePromotion(result);
            return result;
        }

    }
}