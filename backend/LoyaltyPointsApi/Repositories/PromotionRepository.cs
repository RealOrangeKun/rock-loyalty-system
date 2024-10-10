using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Data;
using LoyaltyPointsApi.Models;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace LoyaltyPointsApi.Repositories
{
    public class PromotionRepository(LoyaltyDbContext dbContext,
    ILogger<PromotionRepository> logger) : IPromotionRepository

    {
        public async Task<Promotion?> AddPromotion(Promotion promotion)
        {
            logger.LogInformation("Adding Promotion: {promotion} for restaurant: {restaurantId}", promotion.PromoCode, promotion.RestaurantId);
            await dbContext.Promotions.AddAsync(promotion);
            await dbContext.SaveChangesAsync();
            return promotion;
        }

        public async Task DeletePromotion(Promotion promotion)
        {
            logger.LogInformation("Deleting Promotion: {promotion} for restaurant: {restaurantId}", promotion.PromoCode, promotion.RestaurantId);
            dbContext.Promotions.Remove(promotion);
            await dbContext.SaveChangesAsync();
        }

        public async Task<List<Promotion>> GetAllPromotions()
        {

            logger.LogInformation("Getting all Promotions");
            return await dbContext.Promotions.ToListAsync();
        }

        public async Task<Promotion?> GetPromotion(Promotion promotion)
        {
            logger.LogInformation("Getting Promotion: {promotion} for restaurant: {restaurantId}", promotion.PromoCode, promotion.RestaurantId);
            return await dbContext.Promotions.FirstOrDefaultAsync(r => r.PromoCode == promotion.PromoCode); ;
        }


        public async Task<IPagedList<Promotion>> GetThresholdPromotions(Promotion promotion, int pageNumber, int pageSize)
        {
            logger.LogInformation("Getting Thresholdt Promotions: {promotion} for restaurant: {restaurantId}", promotion.ThresholdId, promotion.RestaurantId);
            var result = await dbContext.Promotions.Where(p => p.ThresholdId == promotion.ThresholdId).ToListAsync();
            return result.ToPagedList(pageNumber, pageSize);
        }

        public async Task<Promotion?> UpdatePromotion(Promotion promotion)
        {
            logger.LogInformation("Updating Promotion: {promotion} for restaurant: {restaurantId}", promotion.PromoCode, promotion.RestaurantId);

            // Check if the entity is already tracked in the local context
            var trackedEntity = dbContext.Promotions.Local
                .FirstOrDefault(p => p.PromoCode == promotion.PromoCode);

            if (trackedEntity != null)
            {
                // Detach the tracked entity to avoid conflicts
                dbContext.Entry(trackedEntity).State = EntityState.Detached;
            }

            // Now, attach and update the promotion entity
            dbContext.Promotions.Attach(promotion);
            dbContext.Entry(promotion).State = EntityState.Modified;

            // Explicitly mark the key fields as unmodified
            dbContext.Entry(promotion).Property(p => p.PromoCode).IsModified = false;
            dbContext.Entry(promotion).Property(p => p.RestaurantId).IsModified = false;

            await dbContext.SaveChangesAsync();
            return promotion;
        }

    }
}