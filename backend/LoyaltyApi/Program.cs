using System.Reflection;
using System.Text;
using DotNetEnv;
using LoyaltyApi.Config;
using LoyaltyApi.Data;
using LoyaltyApi.Repositories;
using LoyaltyApi.Services;
using LoyaltyApi.Utilities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

Env.Load();

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<RockDbContext>(options => options.UseSqlite("Data Source=Dika.db"));

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
builder.Services.Configure<LoyaltyApi.Config.FacebookOptions>(builder.Configuration.GetSection("FacebookOptions"));
builder.Services.Configure<LoyaltyApi.Config.GoogleOptions>(builder.Configuration.GetSection("GoogleOptions"));
// builder.Services.Configure<LoyaltyApi.Config.AppleOptions>(builder.Configuration.GetSection("AppleOptions"));
builder.Services.Configure<API>(builder.Configuration.GetSection("API"));

builder.Services.AddControllers();

builder.Services.AddTransient<ITokenRepository, TokenRepository>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IVoucherRepository, VoucherRepository>();
builder.Services.AddTransient<IVoucherService, VoucherService>();
builder.Services.AddTransient<IRestaurantRepository, RestaurantRepository>();
builder.Services.AddTransient<IRestaurantService, RestaurantService>();
builder.Services.AddTransient<IPointsRepository, PointsRepository>();
builder.Services.AddScoped<IPointsService, PointsService>();
builder.Services.AddScoped<OAuth2Service>();
builder.Services.AddTransient<ApiUtility>();
builder.Services.AddTransient<VoucherUtility>();

// Configure authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    var jwtOptions = builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>();
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
    var facebookOptions = builder.Configuration.GetSection("FacebookOptions").Get<LoyaltyApi.Config.FacebookOptions>();
    options.AppId = facebookOptions?.AppId ?? throw new InvalidOperationException("Facebook app id not found");
    options.AppSecret = facebookOptions?.AppSecret ?? throw new InvalidOperationException("Facebook app secret not found");
    options.CallbackPath = new PathString("/signin-facebook");
})
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    var googleOptions = builder.Configuration.GetSection("GoogleOptions").Get<LoyaltyApi.Config.GoogleOptions>();
    options.ClientId = googleOptions?.ClientId ?? throw new InvalidOperationException("Google Client id not found");
    options.ClientSecret = googleOptions?.ClientSecret ?? throw new InvalidOperationException("Google Client secret not found");
    options.Scope.Add("email");
    options.Scope.Add("profile");
    options.CallbackPath = new PathString("/signin-google");
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Get the path to the XML documentation file
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    // Include the XML documentation in Swagger
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseStatusCodePages(async context =>
{
    if (context.HttpContext.Response.StatusCode == StatusCodes.Status404NotFound)
    {
        // You can render a custom view for 404 errors
        context.HttpContext.Response.ContentType = "text/html";
        await context.HttpContext.Response.WriteAsync("<h1>404 - Page Not Found</h1>");
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
