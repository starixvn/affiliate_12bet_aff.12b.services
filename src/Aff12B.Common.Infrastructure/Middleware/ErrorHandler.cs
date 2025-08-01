using Aff12B.Common.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Aff12B.Common.Infrastructure.Middleware
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

            context.Response.Headers.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
    }
}