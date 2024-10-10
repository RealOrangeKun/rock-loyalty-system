using LoyaltyPointsApi.Data;
using LoyaltyPointsApi.Models;
using LoyaltyPointsApi.Repositories;
using LoyaltyPointsApi.Utilities;
using Microsoft.EntityFrameworkCore;


namespace LoyaltyPointsApi.Services
{

    public class NotifyService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<NotifyService> logger
        ) : BackgroundService

    {
        private Timer timer;
        private readonly HashSet<string> _promotionSet = [];

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Periodic Task Service is starting.");


            timer = new Timer(TriggerTasks, null, TimeSpan.Zero, TimeSpan.FromMinutes(60));

            return Task.CompletedTask;
        }

        private void TriggerTasks(object? state)
        {
            logger.LogInformation("Triggering new tasks at: {time}", DateTimeOffset.Now);

            ProcessTasksAsync().GetAwaiter().GetResult();
        }

        // Make this a proper async Task
        private async Task ProcessTasksAsync()
        {
            try
            {
                using var scope = serviceScopeFactory.CreateScope();

                var restaurantRepository = scope.ServiceProvider.GetRequiredService<IRestaurantRepository>();
                var thresholdRepository = scope.ServiceProvider.GetRequiredService<IThresholdRepository>();
                // Fetch restaurant settings
                List<RestaurantSettings> restaurants = await restaurantRepository.GetAllRestaurants();

                if (restaurants.Count == 0) return;

                List<List<Threshold>> thresholds = [];

                foreach (var restaurant in restaurants)
                {
                    List<Threshold>? restaurantThresholds =
                        await thresholdRepository.GetThresholdsWithPromotions(new Threshold() { RestaurantId = restaurant.RestaurantId });
                    if (restaurantThresholds == null)
                    {
                        continue;
                    }

                    thresholds.Add(restaurantThresholds);
                }
                logger.LogDebug("Thresholds: {thresholds}", thresholds);

                List<Promotion> promotions = [];
                foreach (var restaurantThresholds in thresholds)
                {
                    foreach (var threshold in restaurantThresholds)
                    {
                        foreach (var promotion in threshold.Promotions)
                        {
                            if (promotion.StartDate.Date != DateTime.Now.Date || _promotionSet.Contains(promotion.PromoCode)) continue;
                            promotions.Add(promotion);
                            logger.LogInformation("Adding promotion: {promoCode}", promotion.PromoCode);
                            _promotionSet.Add(promotion.PromoCode);
                        }
                    }
                    logger.LogDebug("Promotions: {promotions}", promotions.Count);

                    foreach (var promotion in promotions)
                    {
                        promotion.StartDate = promotion.StartDate.AddMinutes(2);
                        if (promotion.IsNotified)
                        {
                            logger.LogInformation("Skipping promotion: {promoCode}", promotion.PromoCode);
                            continue;
                        }
                        var delay = promotion.StartDate - DateTime.Now;
                        if (delay.TotalMilliseconds < 0)
                        {
                            logger.LogWarning("Delay in milliseconds is negative: {delay}", delay.TotalMilliseconds);
                            continue;
                        }
                        double delayInMilliseconds = delay.TotalMilliseconds;
                        logger.LogInformation("Delay in milliseconds: {delayInMilliseconds}", delayInMilliseconds);
                        await Task.Run(() => ExecuteDelayedTask(promotion, delay.Seconds));
                        logger.LogInformation("Task scheduled for promotion: {promoCode}", promotion.PromoCode);
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

            var promotionService = scope.ServiceProvider.GetRequiredService<IPromotionService>();
            var loyaltyPointsTransactionRepository =
                scope.ServiceProvider.GetRequiredService<ILoyaltyPointsTransactionRepository>();
            var thresholdService = scope.ServiceProvider.GetRequiredService<IThresholdService>();
            var apiUtility = scope.ServiceProvider.GetRequiredService<ApiUtility>();
            var dbContext = scope.ServiceProvider.GetRequiredService<LoyaltyDbContext>();

            var boundaries =
                await thresholdService.GetThresholdBoundaries(promotion.ThresholdId, promotion.RestaurantId);

            var customerIds =
                await loyaltyPointsTransactionRepository.GetCustomersByRestaurantAndPointsRange(
                    promotion.RestaurantId,
                    boundaries[0], boundaries[1]);
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                foreach (var customerId in customerIds)
                {
                    User user = new()
                    {
                        CustomerId = customerId,
                        RestaurantId = promotion.RestaurantId
                    };
                    User? result =
                        await apiUtility.GetUserAsync(user, await apiUtility.GetApiKey(promotion.RestaurantId.ToString()));
                    logger.LogDebug("User: {user}", user.Email);
                    if (result is null) continue;
                    if (result.Email is null) continue;
                    await NotifyUserAsync(promotion, result);
                    await promotionService.DeletePromotion(promotion.PromoCode);
                    _promotionSet.Remove(promotion.PromoCode);
                }
                await transaction.CommitAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.LogError(ex, "Concurrency conflict while notifying users.");
                foreach (var entry in ex.Entries)
                {
                    if (entry.Entity is Promotion)
                    {
                        var databaseValues = entry.GetDatabaseValues();
                        if (databaseValues == null)
                        {
                            logger.LogWarning("The promotion was deleted by another user.");
                        }
                        else
                        {
                            entry.OriginalValues.SetValues(databaseValues); // Refresh the entity
                        }
                    }

                }
                await transaction.RollbackAsync(); // Rollback on error
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                logger.LogInformation("Finished notifying users about promotion: {promoCode}.", promotion.PromoCode);
            }

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