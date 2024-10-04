using LoyaltyPointsApi.Models;
using LoyaltyPointsApi.Repositories;
using LoyaltyPointsApi.Utilities;

namespace LoyaltyPointsApi.Services
{
    public class NotifyService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<NotifyService> logger
        ) : BackgroundService
    
    {
        private Timer timer;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Periodic Task Service is starting.");


            timer = new Timer(TriggerTasks, null, TimeSpan.Zero, TimeSpan.FromHours(1));

            return Task.CompletedTask;
        }

        private void TriggerTasks(object? state)
        {
            logger.LogInformation("Triggering new tasks at: {time}", DateTimeOffset.Now);

            Task.Run(async () => await ProcessTasksAsync());
        }

        // Make this a proper async Task
        private async Task ProcessTasksAsync()
        {
            try
            {
                using var scope = serviceScopeFactory.CreateScope();
                
                var restaurantRepository = scope.ServiceProvider.GetRequiredService<IRestaurantRepository>();
                var thresholdService = scope.ServiceProvider.GetRequiredService<IThresholdService>();

                // Fetch restaurant settings
                List<RestaurantSettings> restaurants = await restaurantRepository.GetAllRestaurants();

                List<List<Threshold>> thresholds = new();
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

                List<Promotion> promotions = new();
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

                    foreach (var promotion in promotions)
                    {
                        if (promotion.IsNotified || promotion.StartDate < DateTime.Now) continue;
                        var delay = promotion.StartDate - DateTime.Now;
                        double delayInMilliseconds = delay.TotalMilliseconds;
                        await Task.Run(() => ExecuteDelayedTask(promotion, delayInMilliseconds));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while processing tasks.");
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
            using var scope = serviceScopeFactory.CreateScope();
            
            var promotionRepository = scope.ServiceProvider.GetRequiredService<IPromotionRepository>();
            var loyaltyPointsTransactionRepository =
                scope.ServiceProvider.GetRequiredService<ILoyaltyPointsTransactionRepository>();
            var thresholdService = scope.ServiceProvider.GetRequiredService<IThresholdService>();
            var apiUtility = scope.ServiceProvider.GetRequiredService<ApiUtility>();
                
            var boundaries =
                await thresholdService.GetThresholdBoundaries(promotion.ThresholdId, promotion.RestaurantId);

            var customerIds =
                await loyaltyPointsTransactionRepository.GetCustomersByRestaurantAndPointsRange(
                    promotion.RestaurantId,
                    boundaries[0], boundaries[1]);

            foreach (var customerId in customerIds)
            {
                User user = new()
                {
                    CustomerId = customerId,
                    RestaurantId = promotion.RestaurantId
                };
                var result =
                    await apiUtility.GetUserAsync(user, await apiUtility.GetApiKey(user.RestaurantId.ToString()));
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

        private async Task NotifyUserAsync(Promotion promotion, User user)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var restaurantService = scope.ServiceProvider.GetRequiredService<IRestaurantService>();
            var emailUtility = scope.ServiceProvider.GetRequiredService<EmailUtility>();
            var restaurant = await restaurantService.GetRestaurant(promotion.RestaurantId);
            
            string message = $"Hello {user.Name},\n\nWe are excited to share our latest promotion with you! \n\n" +
                             $"Promotion Code: {promotion.PromoCode}\n" +
                             $"Start Date: {promotion.StartDate}\n" +
                             $"End Date: {promotion.EndDate}\n\n" +
                             "Don't miss out on this great offer! Use the promo code at checkout to enjoy discounts.\n\n" +
                             $"Best regards,\n{restaurant.Name}";

            var sent = await emailUtility.SendEmailAsync(user.Email, "Promotion", message, restaurant.Name);

            if (!sent)
            {
                sent = await emailUtility.SendEmailAsync(user.Email, "Promotion", message, restaurant.Name);
                if (!sent) logger.LogWarning($"Email not sent for {user.Email}");
            }
        }

        public async void OnPromotionAdded(object sender, Promotion promotion)
        {
            await NotifyUsers(promotion);
        }
    }
}