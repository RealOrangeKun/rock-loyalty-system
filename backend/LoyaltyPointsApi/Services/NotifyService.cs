using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using LoyaltyPointsApi.Models;
using LoyaltyPointsApi.Repositories;
using LoyaltyPointsApi.Services;
using LoyaltyPointsApi.Utilities;
using System.ComponentModel.DataAnnotations;

namespace LoyaltyPointsApi.Services
{
    public class NotifyService(
        Timer timer,
        ILogger<NotifyService> logger,
        IRestaurantRepository restaurantRepository,
        IThresholdService thresholdService,
        ILoyaltyPointsTransactionRepository loyaltyPointsTransactionRepository,
        ApiUtility apiUtility,
        EmailUtility emailUtility,
        IPromotionRepository promotionRepository
        , IRestaurantService RestaurantService) : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Periodic Task Service is starting.");


            timer = new Timer(TriggerTasks, null, TimeSpan.Zero, TimeSpan.FromHours(1));

            return Task.CompletedTask;
        }

        private async void TriggerTasks(object state)
        {
            logger.LogInformation("Triggering new tasks at: {time}", DateTimeOffset.Now);

            List<RestaurantSettings> restaurants = await restaurantRepository.GetAllRestaurants();

            List<List<Threshold>> thresholds = [];
            foreach (var restaurant in restaurants)
            {
                List<Threshold>? restaurantThresholds =
                    await thresholdService.GetRestaurantThresholds(restaurant.RestaurantId);
                if (restaurantThresholds == null)
                {
                    continue;
                }

                thresholds.Add(restaurantThresholds);
            }
            List<Promotion> promotions = [];
            foreach (var restaurantThresholds in thresholds)
            {
                foreach (var threshold in restaurantThresholds)
                {
                    foreach (var promotion in threshold.Promotions)
                    {
                        if (promotion.StartDate.Date != DateTime.Now.Date) continue;
                        promotions.Add(promotion);
                    }
                }

                var i = 1;
                foreach (var promotion in promotions)
                {
                    if (promotion.IsNotified) continue;
                    if (promotion.StartDate < DateTime.Now) continue;
                    var delay = promotion.StartDate - DateTime.Now;
                    double delayInMilliseconds = delay.TotalMilliseconds;
                    i++;
                    await Task.Run(() => ExecuteDelayedTask(promotion, delayInMilliseconds));
                }
            }
        }


        private async Task ExecuteDelayedTask(Promotion promotion, double delayInSeconds)
        {
            logger.LogInformation("Task scheduled to run in {delayInSeconds} seconds for promotion: {promoCode}.",
                delayInSeconds, promotion.PromoCode);

            await Task.Delay(TimeSpan.FromSeconds(delayInSeconds));

            logger.LogInformation("Executing task for promotion: {promoCode} at: {time}.", promotion.PromoCode,
                DateTimeOffset.Now);

            await NotifyUsers(promotion);
        }

        private async Task NotifyUsers(Promotion promotion)
        {
            logger.LogInformation("Notifying users about promotion: {promoCode}", promotion.PromoCode);

            var boundaries = await thresholdService.GetThresholdBoundaries(promotion.ThresholdId, promotion.RestaurantId);

            var customerIds = await loyaltyPointsTransactionRepository.Zura(promotion.RestaurantId, boundaries[0], boundaries[1]);

            foreach (var customerId in customerIds)
            {
                User user = new()
                {
                    CustomerId = customerId,
                    RestaurantId = promotion.RestaurantId
                };
                var result = await apiUtility.GetUserAsync(user, await apiUtility.GetApiKey(user.RestaurantId.ToString()));
                if (user.Email is null) continue;
                await NotifyUserAsync(promotion, user);
                await promotionRepository.SetPromotionNotified(promotion);
            }

            logger.LogInformation("Finished notifying users about promotion: {promoCode}.", promotion.PromoCode);
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Periodic Task Service is stopping.");
            timer?.Change(Timeout.Infinite, 0);
            return base.StopAsync(stoppingToken);
        }

        public override void Dispose()
        {
            timer?.Dispose();
            base.Dispose();
        }
        public async Task NotifyUserAsync(Promotion promotion, User user)
        {

            var restaurant = await RestaurantService.GetRestaurant(promotion.RestaurantId);
            string Message = $"Hello {user.Name},\n\nWe are excited to share our latest promotion with you! \n\n" +
                $"Promotion Code: {promotion.PromoCode}\n" +
                $"Start Date: {promotion.StartDate}\n" +
                $"End Date: {promotion.EndDate}\n\n" +
                "Don't miss out on this great offer! Use the promo code at checkout to enjoy discounts.\n\n" +
                $"Best regards,\n{restaurant.Name}";

            var sent = await emailUtility.SendEmailAsync(user.Email, "Promotion", Message, restaurant.Name);

            if (!sent)
            {
                sent = await emailUtility.SendEmailAsync(user.Email, "Promotion", Message, restaurant.Name);
                if (!sent) logger.LogWarning($"Email not sent for {user.Email}");
            }
        }

        public async void OnPromotionAdded(Object sender, Promotion promotion)
        {
            await Task.Delay(promotion.StartDate - DateTime.Now);
            await NotifyUsers(promotion);
        }

    }
}