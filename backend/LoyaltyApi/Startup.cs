using System.Diagnostics;
using System.Reflection;
using System.Text;
using DotNetEnv;
using LoyaltyApi.Config;
using LoyaltyApi.Data;
using LoyaltyApi.Models;
using LoyaltyApi.Repositories;
using LoyaltyApi.Services;
using LoyaltyApi.Utilities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LoyaltyApi
{
    public class Startup(IWebHostEnvironment env, IConfiguration configuration)
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));
            services.Configure<Config.FacebookOptions>(configuration.GetSection("FacebookOptions"));
            services.Configure<Config.GoogleOptions>(configuration.GetSection("GoogleOptions"));
            // services.Configure<LoyaltyApi.Config.AppleOptions>(configuration.GetSection("AppleOptions"));
            services.Configure<API>(configuration.GetSection("API"));
            services.Configure<AdminOptions>(configuration.GetSection("AdminOptions"));

            services.AddControllers();

            services.AddTransient<ITokenRepository, TokenRepository>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IVoucherRepository, VoucherRepository>();
            services.AddTransient<IVoucherService, VoucherService>();
            services.AddTransient<IRestaurantRepository, RestaurantRepository>();
            services.AddTransient<IRestaurantService, RestaurantService>();
            services.AddTransient<ICreditPointsTransactionRepository, CreditPointsTransactionRepository>();
            services.AddScoped<OAuth2Service>();
            services.AddTransient<ApiUtility>();
            services.AddTransient<VoucherUtility>();
            services.AddTransient<CreditPointsUtility>();
            services.AddTransient<ICreditPointsTransactionDetailRepository, CreditPointsTransactionDetailRepository>();
            services.AddTransient<ICreditPointsTransactionRepository, CreditPointsTransactionRepository>();
            services.AddTransient<ICreditPointsTransactionService, CreditPointsTransactionService>();
            services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();

            // Database setup
            if (env.IsEnvironment("Testing") || env.IsDevelopment())
            {
                services.AddDbContext<RockDbContext>(options =>
                    options.UseSqlite("Data Source=Dika.db"));
            }

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
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddFacebook(FacebookDefaults.AuthenticationScheme, options =>
            {
                var facebookOptions = configuration.GetSection("FacebookOptions").Get<LoyaltyApi.Config.FacebookOptions>();
                options.AppId = facebookOptions?.AppId ?? throw new InvalidOperationException("Facebook app id not found");
                options.AppSecret = facebookOptions?.AppSecret ?? throw new InvalidOperationException("Facebook app secret not found");
                options.CallbackPath = new PathString("/signin-facebook");
            })
            .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                var googleOptions = configuration.GetSection("GoogleOptions").Get<LoyaltyApi.Config.GoogleOptions>();
                options.ClientId = googleOptions?.ClientId ?? throw new InvalidOperationException("Google Client id not found");
                options.ClientSecret = googleOptions?.ClientSecret ?? throw new InvalidOperationException("Google Client secret not found");
                options.Scope.Add("email");
                options.Scope.Add("profile");
                options.CallbackPath = new PathString("/signin-google");
            });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                // Get the path to the XML documentation file
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                // Include the XML documentation in Swagger
                options.IncludeXmlComments(xmlPath);
            });
        }
        public void Configure(WebApplication app, IWebHostEnvironment env, RockDbContext dbContext)
        {
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
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
        }
        private void AddMigrationsAndUpdateDatabase(RockDbContext dbContext)
        {
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
                    Debug.WriteLine(output);
                }

                if (!string.IsNullOrEmpty(error))
                {
                    Debug.WriteLine(error);
                }
            }

            // Apply the migration
            dbContext.Database.Migrate();
        }
    }
}