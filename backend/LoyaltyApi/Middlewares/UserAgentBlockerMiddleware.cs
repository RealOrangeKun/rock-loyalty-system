namespace LoyaltyApi.Middlewares
{
    public class UserAgentBlockerMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var userAgent = context.Request.Headers.UserAgent.ToString();
            List<string>? allowedUserAgents = configuration.GetSection("AllowedUserAgents").Get<List<string>>() ?? [];

            if (!allowedUserAgents.Contains(userAgent) && allowedUserAgents.Count > 0)
            {
                context.Response.StatusCode = 403;
                return;
            }
            await next(context);
        }
    }
}