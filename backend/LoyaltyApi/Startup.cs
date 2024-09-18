using System.Diagnostics;
using System.Reflection;
using System.Text;
using AspNetCoreRateLimit;
using LoyaltyApi.Config;
using LoyaltyApi.Data;
using LoyaltyApi.Models;
using LoyaltyApi.Repositories;
using LoyaltyApi.Services;
using LoyaltyApi.Utilities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace LoyaltyApi
{
    public class Startup(IWebHostEnvironment env,
    IConfiguration configuration)
    {
        public void ConfigureServices(IServiceCollection services)
        {
            Log.Logger.Information("Setting configurations");

            services.AddHttpContextAccessor();
            services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));
            services.Configure<FacebookOptions>(configuration.GetSection("FacebookOptions"));
            services.Configure<GoogleOptions>(configuration.GetSection("GoogleOptions"));
            services.Configure<API>(configuration.GetSection("API"));
            services.Configure<EmailOptions>(configuration.GetSection("EmailOptions"));
            services.Configure<AdminOptions>(configuration.GetSection("AdminOptions"));
            services.Configure<FrontendOptions>(configuration.GetSection("FrontendOptions"));
            services.AddMemoryCache();
            services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
            services.AddInMemoryRateLimiting();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            Log.Logger.Information("Configuring configurations done");

            Log.Logger.Information("Configuring controllers");
            services.AddControllers();

            Log.Logger.Information("Configuring controllers done");

            Log.Logger.Information("Configuring services in container");

            services.AddTransient<ITokenRepository, TokenRepository>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IVoucherRepository, VoucherRepository>();
            services.AddTransient<IVoucherService, VoucherService>();
            services.AddTransient<IRestaurantRepository, RestaurantRepository>();
            services.AddTransient<IRestaurantService, RestaurantService>();
            services.AddTransient<ICreditPointsTransactionRepository, CreditPointsTransactionRepository>();
            services.AddScoped(provider =>

              new OAuth2Service(new HttpClient())
            );
            services.AddTransient<ApiUtility>();
            services.AddTransient<VoucherUtility>();
            services.AddTransient<CreditPointsUtility>();
            services.AddTransient<TokenUtility>();
            services.AddTransient<EmailUtility>();
            services.AddTransient<ICreditPointsTransactionDetailRepository, CreditPointsTransactionDetailRepository>();
            services.AddTransient<ICreditPointsTransactionRepository, CreditPointsTransactionRepository>();
            services.AddTransient<ICreditPointsTransactionService, CreditPointsTransactionService>();
            services.AddTransient<IPasswordHasher<Password>, PasswordHasher<Password>>();
            services.AddTransient<IPasswordRepository, PasswordRepository>();
            services.AddTransient<IPasswordService, PasswordService>();

            Log.Logger.Information("Configuring services in container done");


            Log.Logger.Information("Configuring database");
            // Database setup
            if (env.IsEnvironment("Testing"))
            {
                Log.Logger.Information("Setting up SQLite database");
                services.AddDbContext<RockDbContext>(options =>
                    options.UseSqlite("Data Source=Dika.db"));
                Log.Logger.Information("Setting up SQLite database done");
            }
            else if (env.IsDevelopment())
            {

                Log.Logger.Information("Setting up MySQL database");

                services.AddDbContext<RockDbContext>(options =>
                {
                    options.UseMySql(configuration.GetSection("ConnectionStrings:DefaultConnection").Value,
                    new MySqlServerVersion(new Version(8, 0, 29)));
                });
                Log.Logger.Information("Setting up MySQL database done");
            }

            Log.Logger.Information("Configuring database done");

            Log.Logger.Information("Configuring authentication");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>();
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions?.SigningKey ?? throw new InvalidOperationException("JWT signing key not found"))),
                };
            });
            Log.Logger.Information("Configuring authentication done");

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                // Get the path to the XML documentation file
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                // Include the XML documentation in Swagger
                options.IncludeXmlComments(xmlPath);
            });
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularClient", builder =>
                {
                    var frontendOptions = configuration.GetSection("FrontendOptions").Get<FrontendOptions>() ?? throw new ArgumentException("Frontend options missing");
                    builder.WithOrigins(frontendOptions.BaseUrl)
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .AllowAnyMethod();
                });
            });
        }
        public void Configure(WebApplication app, IWebHostEnvironment env, RockDbContext dbContext)
        {
            Log.Logger.Information("Configuring web application");

            app.UseIpRateLimiting();

            if (env.IsDevelopment() || env.IsEnvironment("Testing"))
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            if (env.IsEnvironment("Testing"))
            {

                AddMigrationsAndUpdateDatabase(dbContext);
            }

            app.UseStatusCodePages(async context =>
            {
                if (context.HttpContext.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    // You can render a custom view for 404 errors
                    context.HttpContext.Response.ContentType = "text/html";
                    await context.HttpContext.Response.WriteAsync("<h1>404 - Page Not Found</h1>");
                }
            });
            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseCors("AllowAngularClient");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            Log.Logger.Information("Configuring web application done");
        }
        private void AddMigrationsAndUpdateDatabase(RockDbContext dbContext)
        {
            Log.Logger.Information("Adding migrations and updating database");
            if (File.Exists("Dika.db")) File.Delete("Dika.db");

            dbContext.Database.EnsureCreated();

            // Get the path to the migrations folder
            var migrationsFolderPath = Path.Combine(env.ContentRootPath, "Migrations");

            // Check if the Migrations folder exists, if not, create it
            if (Directory.Exists(migrationsFolderPath))
                Directory.Delete(migrationsFolderPath, true);
            // Run the command to add a migration
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "ef migrations add InitialMigration --context RockDbContext",
                WorkingDirectory = env.ContentRootPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processStartInfo))
            {
                process?.WaitForExit();

                var output = process?.StandardOutput.ReadToEnd();
                var error = process?.StandardError.ReadToEnd();

                if (!string.IsNullOrEmpty(output))
                {
                    Log.Logger.Debug("Process output: {Output}", output);
                }

                if (!string.IsNullOrEmpty(error))
                {
                    Log.Logger.Error("Process error: {Error}", error);
                }
            }

            // Apply the migration
            dbContext.Database.Migrate();
            Log.Logger.Information("Adding migrations and updating database done");
        }
    }
}