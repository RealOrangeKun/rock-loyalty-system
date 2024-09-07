using DotNetEnv;
using LoyaltyApi;
using LoyaltyApi.Data;
using Serilog;

Env.Load();


var builder = WebApplication.CreateBuilder();
Log.Logger = new LoggerConfiguration()
           .ReadFrom.Configuration(builder.Configuration)
           .Enrich.FromLogContext()
           .WriteTo.Console()
           .WriteTo.File("Logs/LoyaltyApi-{Date}.log", rollingInterval: RollingInterval.Infinite)
           .CreateLogger();

builder.Host.UseSerilog();
builder.Logging.ClearProviders();
builder.Logging.AddConfiguration(builder.Configuration);
builder.Logging.AddSerilog();

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