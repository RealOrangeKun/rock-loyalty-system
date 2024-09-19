using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyPointsApi.RequestModels
{
    public class UserRequestModel
    {
        public required int CustomerId{get; set;}
        public required int RestaurantId{get; set;}
    }
}