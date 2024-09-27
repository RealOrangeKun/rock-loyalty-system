using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using LoyaltyPointsApi.Models;
using LoyaltyPointsApi.Repositories;
using LoyaltyPointsApi.Services;

public class NotifyService(Timer timer, ILogger<NotifyService> logger, 
IRestaurantRepository restaurantRepository,
IThresholdService thresholdService,
ILoyaltyPointsTransactionService loyaltyPointsTransactionService) : BackgroundService
{
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Periodic Task Service is starting.");
        
       
        timer = new Timer(TriggerTasks, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));

        return Task.CompletedTask;
    }

    private async void TriggerTasks(object state)
    {
        logger.LogInformation("Triggering new tasks at: {time}", DateTimeOffset.Now);
        
        List<RestaurantSettings> restaurants = await restaurantRepository.GetAllRestaurants();
    
        List<List<Threshold>> thresholds= [];
        foreach(var restaurant in restaurants){
            List<Threshold>? restaurantThresholds = await thresholdService.GetRestaurantThresholds(restaurant.RestaurantId);
            if(restaurantThresholds == null){
                continue;
                }
            thresholds.Add(restaurantThresholds);
        }
        List<Promotion> promotions = [];
        foreach(var restaurantThresholds in thresholds){
            foreach(var threshold in restaurantThresholds){
                 foreach(var p in threshold.Promotions){
                     promotions.Add(p);
                 }
            }
            foreach(var promotion in promotions){
                if(promotion.IsNotified) continue;
                    
            }
        }       
    }

   
    private async Task ExecuteDelayedTask(int taskId, int delayInSeconds)
    {
        logger.LogInformation("Task {taskId} scheduled to run in {delayInSeconds} seconds.", taskId, delayInSeconds);
        await Task.Delay(TimeSpan.FromSeconds(delayInSeconds));
        logger.LogInformation("Task {taskId} is executing at: {time}", taskId, DateTimeOffset.Now);
       
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
}
