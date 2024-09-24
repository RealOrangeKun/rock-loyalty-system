using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyPointsApi.Utilities
{
    public class ApiKeyUtility
    {
        public static string GenerateApiKey() => Guid.NewGuid().ToString();
    }
}