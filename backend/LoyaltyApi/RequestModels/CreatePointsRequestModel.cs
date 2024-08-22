using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyApi.RequestModels
{
    public class CreatePointsRequestModel
    {
        public int CustomerId { get; set; }
        public int RestaurantId { get; set; }
        public int TransactionId { get; set; }
        public int CreditPoints { get; set; }
    }
}