using LoyaltyPointsApi.Events;
using LoyaltyPointsApi.Models;
using LoyaltyPointsApi.Repositories;
using LoyaltyPointsApi.RequestModels;
using X.PagedList;

namespace LoyaltyPointsApi.Services
{
    public class PromotionService(
        IPromotionRepository promotionRepository,
        PromotionAddedEvent promotionAddedEvent,
        ILogger<PromotionService> logger) : IPromotionService
    {
        public async Task<Promotion?> AddPromotion(AddPromotionRequestModel promotion)
        {
            logger.LogInformation("Adding Promotion: {promotion} for restaurant: {restaurantId}", promotion.PromoCode,
                promotion.RestaurantId);
            Promotion promotionModel = new()
            {
                RestaurantId = promotion.RestaurantId,
                PromoCode = promotion.PromoCode,
                ThresholdId = promotion.ThresholdId,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate
            };
            var result = await promotionRepository.AddPromotion(promotionModel);
            if (result == null) return null;
            if (!result.StartDate.Date.Equals(DateTime.Now.Date)) return result;
            promotionAddedEvent.NotifyCustomers(result);
            return result;
        }

        public async Task DeletePromotion(string promoCode)
        {
            logger.LogInformation("Deleting Promotion: {promoCode}", promoCode);
            Promotion promotion = new()
            {
                PromoCode = promoCode
            };
            Promotion? existingPromotion = await promotionRepository.GetPromotion(promotion) ?? throw new Exception();
            await promotionRepository.DeletePromotion(existingPromotion);
        }


        public async Task<Promotion?> GetPromotion(string promoCode)
        {
            logger.LogInformation("Getting Promotion: {promoCode}", promoCode);
            Promotion promotion = new()
            {
                PromoCode = promoCode
            };
            var result = await promotionRepository.GetPromotion(promotion);

            return result;
        }

        public async Task<IPagedList<Promotion>> GetThresholdPromotions(int thresholdId, int pageNumber, int pageSize)
        {
            logger.LogInformation("Getting Threshold Promotions: {thresholdId}", thresholdId);
            Promotion promotion = new()
            {
                ThresholdId = thresholdId
            };
            return await promotionRepository.GetThresholdPromotions(promotion, pageNumber, pageSize);
        }


        public async Task<Promotion?> UpdatePromotion(string promoCode, UpdatePromotionRequestModel promotion,
            int restaurantId)
        {
            logger.LogInformation("Updating Promotion: {promoCode} for restaurant: {restaurantId}", promoCode,
                restaurantId);
            Promotion existingPromotion = new()
            {
                PromoCode = promoCode,
                RestaurantId = restaurantId
            };
            var result = await promotionRepository.GetPromotion(existingPromotion);
            if (result == null) return null;
            result.ThresholdId = promotion.ThresholdId;
            await promotionRepository.UpdatePromotion(result);
            return result;
        }
        public async Task SetPromotionNotified(string promoCode)
        {
            logger.LogInformation("Setting Promotion: {promoCode} as notified", promoCode);
            Promotion promotion = new()
            {
                PromoCode = promoCode,
            };
            Promotion existingPromotion = await promotionRepository.GetPromotion(promotion) ?? throw new Exception();
            existingPromotion.IsNotified = true;
            await promotionRepository.UpdatePromotion(promotion);
        }
    }
}