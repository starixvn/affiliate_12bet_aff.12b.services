namespace Aff12B.CronJobs.Host.Extensions
{
    public static class MiddlewareExtensions
    {
        public static void UsePing(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/ping"))
                {
                    return;
                }
                await next();
            });
        }
    }
}
