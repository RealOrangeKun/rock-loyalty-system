using System.Reflection;
using System.Text;
using DotNetEnv;
using LoyaltyApi.Config;
using LoyaltyApi.Repositories;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

Env.Load();

var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
builder.Services.Configure<LoyaltyApi.Config.FacebookOptions>(builder.Configuration.GetSection("FacebookOptions"));
builder.Services.Configure<LoyaltyApi.Config.GoogleOptions>(builder.Configuration.GetSection("GoogleOptions"));


builder.Services.AddControllers();

builder.Services.AddTransient<ITokenRepository, TokenRepository>();
builder.Services.AddTransient<ITokenService, TokenService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
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
.AddCookie()
.AddFacebook(FacebookDefaults.AuthenticationScheme, options =>
{
    var facebookOptions = builder.Configuration.GetSection("FacebookOptions").Get<LoyaltyApi.Config.FacebookOptions>();
    options.AppId = facebookOptions?.AppId ?? throw new InvalidOperationException("Facebook app id not found");
    options.AppSecret = facebookOptions?.AppSecret ?? throw new InvalidOperationException("Facebook app secret not found");
    options.Scope.Add("email");
    options.Scope.Add("public_profile");
    options.Fields.Add("name");
    options.Fields.Add("email");
    options.CallbackPath = new PathString("/signin-facebook");
}).AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
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

var app = builder.Build();

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