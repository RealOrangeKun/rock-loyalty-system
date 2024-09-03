using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LoyaltyApi.Config;
using LoyaltyApi.Data;
using LoyaltyApi.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sprache;

namespace LoyaltyApi.Repositories
{
    public class TokenRepository(RockDbContext dbContext,
    IOptions<JwtOptions> jwtOptions) : ITokenRepository
    {
        public string GenerateAccessToken(Token token)
        {
            JwtSecurityToken generatedToken = GenerateToken(token);
            return new JwtSecurityTokenHandler().WriteToken(generatedToken);
        }

        private JwtSecurityToken GenerateToken(Token token)
        {
            var claims = new[]{
                new Claim(JwtRegisteredClaimNames.Sub,token.CustomerId.ToString()),
                new Claim("restaurantId", token.RestaurantId.ToString()),
                new Claim("role", token.Role.ToString())
            };
            var signingKey = jwtOptions.Value.SigningKey.ToString();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var generatedToken = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: token.TokenType != TokenType.RefreshToken ? DateTime.Now.AddMinutes(jwtOptions.Value.ExpirationInMinutes) : DateTime.Now.AddMonths(6),
                signingCredentials: creds
            );
            return generatedToken;
        }

        public bool ValidateRefreshToken(Token token)
        {
            return ValidateToken(token)
            && dbContext.Tokens.Any(t => t.TokenValue == token.TokenValue && t.TokenType == TokenType.RefreshToken);
        }

        public bool ValidateToken(Token token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtOptions.Value.SigningKey);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
            tokenHandler.ValidateToken(token.TokenValue, validationParameters, out SecurityToken validatedToken);
            return validatedToken != null;
        }

        public async Task<string> GenerateRefreshTokenAsync(Token token)
        {
            JwtSecurityToken generatedToken = GenerateToken(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            string valueToken = tokenHandler.WriteToken(generatedToken).ToString();
            int subject = int.Parse(tokenHandler.ReadJwtToken(valueToken).Claims.First(claim => claim.Type == "sub").Value);
            DateTime expiration = tokenHandler.ReadJwtToken(valueToken).ValidTo;
            int restaurantId = int.Parse(tokenHandler.ReadJwtToken(valueToken).Claims.First(claim => claim.Type == "restaurantId").Value);
            var refreshToken = new Token
            {
                TokenValue = valueToken,
                CustomerId = subject,
                RestaurantId = restaurantId,
                TokenType = TokenType.RefreshToken
            };
            await dbContext.Tokens.AddAsync(refreshToken);
            await dbContext.SaveChangesAsync();
            return refreshToken.TokenValue;
        }

        public async Task<string> GenerateForgotPasswordTokenAsync(Token token)
        {
            JwtSecurityToken generatedToken = GenerateToken(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            string valueToken = tokenHandler.WriteToken(generatedToken).ToString();
            int subject = int.Parse(tokenHandler.ReadJwtToken(valueToken).Claims.First(claim => claim.Type == "sub").Value);
            DateTime expiration = tokenHandler.ReadJwtToken(valueToken).ValidTo;
            int restaurantId = int.Parse(tokenHandler.ReadJwtToken(valueToken).Claims.First(claim => claim.Type == "restaurantId").Value);
            var forgotPasswordToken = new Token
            {
                TokenValue = valueToken,
                CustomerId = subject,
                RestaurantId = restaurantId,
                TokenType = TokenType.ForgotPasswordToken
            };
            await dbContext.Tokens.AddAsync(forgotPasswordToken);
            await dbContext.SaveChangesAsync();
            return forgotPasswordToken.TokenValue;
        }

        public async Task<string> GenerateConfirmEmailTokenAsync(Token token)
        {
            JwtSecurityToken generatedToken = GenerateToken(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            string valueToken = tokenHandler.WriteToken(generatedToken).ToString();
            int subject = int.Parse(tokenHandler.ReadJwtToken(valueToken).Claims.First(claim => claim.Type == "sub").Value);
            DateTime expiration = tokenHandler.ReadJwtToken(valueToken).ValidTo;
            int restaurantId = int.Parse(tokenHandler.ReadJwtToken(valueToken).Claims.First(claim => claim.Type == "restaurantId").Value);
            var confirmEmailToken = new Token
            {
                TokenValue = valueToken,
                CustomerId = subject,
                RestaurantId = restaurantId,
                TokenType = TokenType.ConfirmEmail
            };
            await dbContext.Tokens.AddAsync(confirmEmailToken);
            await dbContext.SaveChangesAsync();
            return confirmEmailToken.TokenValue;

        }

        public bool ValidateConfirmEmailToken(Token token)
        {
            return ValidateToken(token)
            && dbContext.Tokens.Any(t => t.TokenValue == token.TokenValue && t.TokenType == TokenType.ConfirmEmail);
        }
    }
}