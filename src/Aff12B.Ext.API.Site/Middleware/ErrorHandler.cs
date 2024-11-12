
using Fanex.Data;
using MySqlConnector;
using System.Net;
using System.Text;

namespace Aff12B.Ext.API.Site.Middleware
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
            var errorMsg = GenerateMessage(exception, context);
            _logger.LogCritical(exception, errorMsg);
            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }

        private string GenerateMessage(Exception exception, HttpContext context)
        {
            var messageBuilder = new StringBuilder();

            messageBuilder.AppendLine($"ErrMsg: {exception.Message}");

            if (context != null)
            {
                AddRequestInfo(context, messageBuilder);
            }

            AddDatabaseInfo(messageBuilder, exception);

            const int limitedNumOfInnerException = 3;
            AddExceptionInfo(messageBuilder, exception, limitedNumOfInnerException);

            return messageBuilder.ToString();
        }

        private static void AddRequestInfo(HttpContext context, StringBuilder builder)
        {
            builder.AppendLine();
            builder.AppendLine("REQUEST INFO");

            var webInfo = GetRequestInfo(context);

            foreach (var item in webInfo)
            {
                builder.AppendLine($"{item.Key}: {item.Value}");
            }
        }

        private static void AddDatabaseInfo(StringBuilder builder, Exception exception)
        {
            builder.AppendLine();
            builder.AppendLine("DATABASE INFO");

            var dbInfo = GetDbExceptionInfo(exception);

            foreach (var item in dbInfo)
            {
                builder.AppendLine($"{item.Key}: {item.Value}");
            }
        }

        private static void AddExceptionInfo(StringBuilder builder, Exception exception, int limitedInnerException)
        {
            builder.AppendLine();
            builder.AppendLine("EXCEPTION INFO");
            builder.AppendLine($"Message: {exception.Message}");
            builder.AppendLine($"Source: {exception.Source}");
            builder.AppendLine($"TargetSite: {exception.TargetSite}");
            builder.AppendLine($"StackTrace: {exception.StackTrace}");

            if (exception.InnerException != null && limitedInnerException > 0)
            {
                AddExceptionInfo(builder, exception.InnerException, limitedInnerException - 1);
            }
        }

        private static IDictionary<string, object> GetDbExceptionInfo(Exception exception)
        {
            var dbInfo = new Dictionary<string, object>();
            BindFanexDataExceptionInfo(exception, dbInfo);
            return dbInfo;
        }

        private static void BindFanexDataExceptionInfo(Exception exception, IDictionary<string, object> dbInfo)
        {
            if (exception is ObjectDbException fanexDataException)
            {

                var connString = new MySqlConnectionStringBuilder(fanexDataException.ConnectionString);

                dbInfo["Server"] = connString.Server;
                dbInfo["DbName"] = fanexDataException.DbName;
                dbInfo["SpName"] = fanexDataException.SpName;
                dbInfo["UserId"] = connString.UserID;
                dbInfo["Parameters"] = GetParameters(fanexDataException.Parameters);
                dbInfo["CommandTimeout"] = fanexDataException.CommandTimeout;
            }
        }

        private static string GetParameters(IDictionary<string, object> parameters)
           => parameters?.Count > 0
               ? parameters.Select(parameter => $"{parameter.Key}={parameter.Value}")
                        .Aggregate((firstPair, secondPair) => $"{firstPair};{secondPair}")
               : string.Empty;


        private static IDictionary<string, object> GetRequestInfo(HttpContext context)
        {
            var request = context.Request;
            return request == null
                    ? new Dictionary<string, object>()
                    : new Dictionary<string, object>
                    {
                        [nameof(request.Host)] = request?.Host,
                        [nameof(request.Path)] = request?.Path,
                        ["Remote IP"] = context.Connection?.RemoteIpAddress
                    };
        }
    }
}
