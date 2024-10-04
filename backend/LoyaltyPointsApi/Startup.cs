using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Config;
using LoyaltyPointsApi.Data;
using LoyaltyPointsApi.Events;
using LoyaltyPointsApi.Middlewares;
using LoyaltyPointsApi.Repositories;
using LoyaltyPointsApi.Services;
using LoyaltyPointsApi.Utilities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LoyaltyPointsApi
{
    public class Startup(IWebHostEnvironment environment,
        IConfiguration configuration)
    {
        public void ConfigureServices(IServiceCollection services)
        {
            Log.Logger.Information("Applying configurations");
            // Adding configurations
            services.Configure<AdminOptions>(configuration.GetSection("AdminOptions"));
            services.Configure<ApiOptions>(configuration.GetSection("ApiOptions"));
            Log.Logger.Information("Configurations applied");

            Log.Logger.Information("Adding services");
            // Adding db context
            services.AddDbContext<LoyaltyDbContext>(options =>
            {
                options.UseSqlite("Data Source=Zura.db");
            });

            // Adding services
            services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
            services.AddScoped<IApiKeyService, ApiKeyService>();
            services.AddScoped<ApiUtility>();
            services.AddTransient<EmailUtility>();
            services.AddScoped<IRestaurantRepository, RestaurantRepository>();
            services.AddScoped<IRestaurantService, RestaurantService>();
            services.AddScoped<IThresholdRepository, ThresholdRepository>();
            services.AddScoped<IThresholdService, ThresholdService>();
            services.AddScoped<ILoyaltyPointsTransactionRepository, LoyaltyPointsTransactionRepository>();
            services.AddScoped<ILoyaltyPointsTransactionService, LoyaltyPointsTransactionService>();
            services.AddScoped<IPromotionRepository, PromotionRepository>();
            services.AddScoped<IPromotionService, PromotionService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddSingleton<NotifyService>();
            services.AddSingleton<PromotionAddedEvent>();
            // services.AddScoped<UserService>();
            Log.Logger.Information("Services added");

            Log.Logger.Information("Adding middlewares");
            // Adding controllers and swagger
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            Log.Logger.Information("Middleware added");

        }
        public void Configure(WebApplication app)
        {
            Log.Logger.Information("Configuring app");
            var notificationService = app.Services.GetService<NotifyService>() ?? throw new Exception("NotifyService is null");
            var addedEvent = app.Services.GetService<PromotionAddedEvent>() ?? throw new Exception("PromotionAddedEvent is null");
            addedEvent.PromotionAdded += notificationService.OnPromotionAdded;
            if (environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            if (environment.IsProduction())
                app.UseMiddleware<ApiKeyValidatorMiddleware>();

            Log.Logger.Information("App configured");

        }

    }
}