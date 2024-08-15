using System.Reflection;
using System.Text;
using DotNetEnv;
using LoyaltyApi.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

Env.Load();

var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));


builder.Services.AddControllers();

builder.Services.AddAuthentication()
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

app.Run();