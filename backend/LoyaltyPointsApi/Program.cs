using DotNetEnv;
using LoyaltyPointsApi;
using Serilog;


Env.Load();

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
           .ReadFrom.Configuration(builder.Configuration)
           .Enrich.FromLogContext()
           .WriteTo.Console()
           .WriteTo.File("Logs/LoyaltyPointsApi-{Date}.log", rollingInterval: RollingInterval.Day)
           .CreateLogger();

builder.Host.UseSerilog();
builder.Logging.ClearProviders();
builder.Logging.AddConfiguration(builder.Configuration);
builder.Logging.AddSerilog();

var startup = new Startup(builder.Environment, builder.Configuration);
startup.ConfigureServices(builder.Services);
var app = builder.Build();
startup.Configure(app);
app.Run();