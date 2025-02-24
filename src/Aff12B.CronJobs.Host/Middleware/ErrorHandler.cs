using Aff12B.CronJobs.Infrastructure.Helpers;
using System.Net;

namespace Aff12B.CronJobs.Host.Middleware
{
    public class ErrorHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandler> _logger;

        public ErrorHandler(RequestDelegate next, ILogger<ErrorHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception exception)
            {
                HandleError(context, exception);
            }
        }

        private void HandleError(HttpContext context, Exception exception)
        {
            var errorMsg = ErrorMessageHelper.GenerateMessage(exception, context);
            _logger.LogCritical(exception, errorMsg);
            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }

    }
}
