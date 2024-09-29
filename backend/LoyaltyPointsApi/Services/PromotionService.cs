using LoyaltyPointsApi.Events;
using LoyaltyPointsApi.Models;
using LoyaltyPointsApi.Repositories;
using LoyaltyPointsApi.RequestModels;

namespace LoyaltyPointsApi.Services
{
    public class PromotionService(IPromotionRepository promotionRepository,
    PromotionAddedEvent promotionAddedEvent) : IPromotionService
    {
        public async Task<Promotion?> AddPromotion(AddPromotionRequestModel promotion)
        {
            Promotion promotionModel = new()
            {
                RestaurantId = promotion.RestaurantId,
                PromoCode = promotion.PromoCode,
                ThresholdId = promotion.ThresholdId,
            };
            var result = await promotionRepository.AddPromotion(promotionModel);
            if (result == null) return null;
            if (!result.StartDate.Date.Equals(DateTime.Now.Date)) return result;
            promotionAddedEvent.NotifyCustomers(result);
            return result;
        }

        public async Task DeletePromotion(string promoCode)
        {
            Promotion promotion = new()
            {
                PromoCode = promoCode
            };
            await promotionRepository.DeletePromotion(promotion);

            Promotion result = await promotionRepository.GetPromotion(promotion) ?? throw new Exception("Promotion not found");

            await promotionRepository.DeletePromotion(result);
        }



        public async Task<Promotion?> GetPromotion(string promoCode)
        {
            Promotion promotion = new()
            {
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


        public async Task<Promotion?> UpdatePromotion(string promoCode, UpdatePromotionRequestModel promotion, int restaurantId)
        {
            Promotion existingPromotion = new()
            {
                PromoCode = promoCode,
                RestaurantId = restaurantId
            };
            var result = await promotionRepository.GetPromotion(existingPromotion);

            if (result == null)
            {
                throw new Exception("Promotion not found");
            }

            result.ThresholdId = promotion.ThresholdId;
            await promotionRepository.UpdatePromotion(result);
            return result;
        }

    }
}