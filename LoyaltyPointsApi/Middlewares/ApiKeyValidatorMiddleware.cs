using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace LoyaltyPointsApi.Middlewares
{
    public class ApiKeyValidatorMiddleware(RequestDelegate next,
    ILogger<ApiKeyValidatorMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var excludedController = "ApiKey";
            var controllerActionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
           logger.LogCritical(controllerActionDescriptor?.ControllerName);

            if (controllerActionDescriptor is not null && String.Compare(controllerActionDescriptor.ControllerName, excludedController, StringComparison.OrdinalIgnoreCase) == 0)
            {
                await next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue("X-ApiKey", out var apiKey))
            {
                context.Response.StatusCode = 401;
                return;
            }

            await next(context);
        }
    }
}