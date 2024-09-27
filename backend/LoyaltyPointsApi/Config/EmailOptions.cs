using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyPointsApi.Config
{
    public class EmailOptions
    {
        public required string BaseUrl { get; set; }

        public required string Key { get; set; }
    }
}