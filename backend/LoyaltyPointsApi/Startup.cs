using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Config;
using LoyaltyPointsApi.Data;
using LoyaltyPointsApi.Middlewares;
using LoyaltyPointsApi.Repositories;
using LoyaltyPointsApi.Services;
using LoyaltyPointsApi.Utilities;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyPointsApi
{
    public class Startup(IWebHostEnvironment environment,
        IConfiguration configuration)
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Adding configurations
            services.Configure<AdminOptions>(configuration.GetSection("AdminOptions"));
            services.Configure<ApiOptions>(configuration.GetSection("ApiOptions"));

            // Adding db context
            services.AddDbContext<LoyaltyDbContext>(options =>
            {
                options.UseSqlite("Data Source=Zura.db");
            });

            // Adding services
            services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
            services.AddScoped<IApiKeyService, ApiKeyService>();
            services.AddScoped<ApiUtility>();
            services.AddScoped<IRestaurantRepository, RestaurantRepository>();
            services.AddScoped<IRestaurantService, RestaurantService>();
            services.AddScoped<IThresholdRepository, ThresholdRepository>();
            services.AddScoped<IThresholdService, ThresholdService>();
            services.AddScoped<ILoyaltyPointsTransactionRepository, LoyaltyPointsTransactionRepository>();
            services.AddScoped<ILoyaltyPointsTransactionService, LoyaltyPointsTransactionService>();
            services.AddScoped<IPromotionRepository , PromotionRepository>();
            services.AddScoped<IPromotionService, PromotionService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            // services.AddScoped<UserService>();


            // Adding controllers and swagger
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

        }
        public void Configure(WebApplication app)
        {

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
            // app.UseMiddleware<ApiKeyValidatorMiddleware>();

        }

    }
}