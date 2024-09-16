using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyPointsApi.RequestModels
{
    public class LoginRequestBody
    {
        public required int RestaurantId { get; set; }
    }
}