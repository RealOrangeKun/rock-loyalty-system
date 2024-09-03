using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LoyaltyApi.Models;

namespace LoyaltyApi.Utilities
{
    public class TokenUtility
    {
        public Token ReadToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var customerId = int.Parse(jwtToken.Claims.First(claim => claim.Type == "sub").Value);
            var restaurantId = int.Parse(jwtToken.Claims.First(claim => claim.Type == "restaurantId").Value);
            return new Token
            {
                CustomerId = customerId,
                RestaurantId = restaurantId,
                Role = Role.User
            };
        }
    }
}