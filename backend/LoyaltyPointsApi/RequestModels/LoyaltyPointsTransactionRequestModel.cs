using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Models;

namespace LoyaltyPointsApi.RequestModels
{
    public class LoyaltyPointsTransactionRequestModel
    {

        public int ReceiptId { get; set; }

        public required int CustomerId { get; set; }

        public required int RestaurantId { get; set; }


        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;


        public RestaurantSettings Restaurant { get; set; }
    }
}