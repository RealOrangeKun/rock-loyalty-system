using DotNetEnv;
using LoyaltyApi;
using LoyaltyApi.Data;

Env.Load();

var builder = WebApplication.CreateBuilder();

var startup = new Startup(builder.Environment, builder.Configuration);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<RockDbContext>();

    // Pass the DbContext to Startup's Configure method
    startup.Configure(app, app.Environment, dbContext);
}

app.Run();