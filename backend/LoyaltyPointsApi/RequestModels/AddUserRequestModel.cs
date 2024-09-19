using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyPointsApi.RequestModels
{
    public class AddUserRequestModel
    {
      public int CustomerId { get; set; }
        public int RestaurantId { get; set; }

        public string Rank { get; set; }

        public int LoyaltyPoints { get; set; }  
    }
}