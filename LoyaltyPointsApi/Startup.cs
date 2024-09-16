using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Data;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyPointsApi
{
    public class Startup(IWebHostEnvironment environment,
        IConfiguration configuration)
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddDbContext<LoyaltyDbContext>(options=>{
                options.UseSqlite("Data Source=Zura.db");
            });
        }
        public void Configure(WebApplication app)
        {

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();


        }

    }
}