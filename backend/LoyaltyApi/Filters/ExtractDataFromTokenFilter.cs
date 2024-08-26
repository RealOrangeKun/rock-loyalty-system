using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LoyaltyApi.Filters
{
    public class ExtractDataFromTokenFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            string? customerId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? context.HttpContext.User.FindFirstValue("sub");
            string? restaurantId = context.HttpContext.User.FindFirstValue("restaurantId");
            if (string.IsNullOrEmpty(customerId) || string.IsNullOrEmpty(restaurantId)) return;
            context.HttpContext.Items.Add("customerId", customerId);
            context.HttpContext.Items.Add("restaurantId", restaurantId);
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do nothing
        }
    }
}