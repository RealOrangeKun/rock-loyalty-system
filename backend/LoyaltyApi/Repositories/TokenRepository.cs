using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LoyaltyApi.Config;
using LoyaltyApi.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sprache;

namespace LoyaltyApi.Repositories
{
    public class TokenRepository(IOptions<JwtOptions> jwtOptions) : ITokenRepository
    {
        public string GenerateAccessToken(int userId, int restaurantId)
        {
            JwtSecurityToken token = GenerateToken(userId, restaurantId, TokenType.AccessToken);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private JwtSecurityToken GenerateToken(int customerId, int restaurantId, TokenType type)
        {
            var claims = new[]{
                new Claim(JwtRegisteredClaimNames.Sub,customerId.ToString()),
                new Claim("restaurantId", restaurantId.ToString())
            };
            var signingKey = jwtOptions.Value.SigningKey.ToString();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddMinutes(jwtOptions.Value.ExpirationInMinutes),
                signingCredentials: creds
            );
            return token;
        }

        public bool ValidateRefreshToken(string token)
        {
            return ValidateToken(token)
            // && context.Tokens.Any(t => t.TokenValue == token && t.TokenType == TokenType.RefreshToken)
            ;
        }

        public bool ValidateToken(string token)
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
            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            return validatedToken != null;
        }

        public async Task<string> GenerateRefreshTokenAsync(int customerId, int restaurantId)
        {
            JwtSecurityToken token = GenerateToken(customerId, restaurantId, TokenType.RefreshToken);
            var tokenHandler = new JwtSecurityTokenHandler();
            string valueToken = tokenHandler.WriteToken(token).ToString();
            int subject = int.Parse(tokenHandler.ReadJwtToken(valueToken).Claims.First(claim => claim.Type == "sub").Value);
            DateTime expiration = tokenHandler.ReadJwtToken(valueToken).ValidTo;
            var refreshToken = new Token
            {
                TokenValue = valueToken,
                CustomerId = customerId,
                RestaurantId = restaurantId,
                TokenType = TokenType.RefreshToken
            };
            // context.Tokens.Add(refreshToken);
            // context.SaveChanges();
            return refreshToken.TokenValue;
        }
    }
}