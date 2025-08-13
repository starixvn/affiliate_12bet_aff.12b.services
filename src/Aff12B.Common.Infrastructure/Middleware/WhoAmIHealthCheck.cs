using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net;
using System.Net.Sockets;

namespace Aff12B.Common.Infrastructure.Middleware
{
    public class WhoAmIHealthCheck : IHealthCheck
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHostEnvironment _env;

        public WhoAmIHealthCheck(IHttpContextAccessor httpContextAccessor, IHostEnvironment env)
        {
            _httpContextAccessor = httpContextAccessor;
            _env = env;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            IDictionary<string, object> result = new Dictionary<string, object>
            {
                { "Env", _env.EnvironmentName }
            };
            await AddServerIPInfo(result);
            AddExtraServerInfo(result);

            return HealthCheckResult.Healthy("Server Info", new ReadOnlyDictionary<string, object>(result));
        }

        private static async Task AddServerIPInfo(IDictionary<string, object> result)
        {
            var hostName = Dns.GetHostName();
            result.Add("Host Name", hostName);

            var serverIps = await Dns.GetHostAddressesAsync(hostName).ConfigureAwait(false);
            for (var i = 0; i < serverIps.Length; i++)
            {
                if (serverIps[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    result.Add($"IP{i}", serverIps[i].ToString());
                }
            }
        }

        private void AddExtraServerInfo(IDictionary<string, object> result)
        {
            var context = _httpContextAccessor.HttpContext;
            result.Add("Local Ip", context.Connection.LocalIpAddress.ToString());
            result.Add("Local Port", context.Connection.LocalPort);
            result.Add("Client Ip", context.Connection.RemoteIpAddress.ToString());
            result.Add("Server Time", DateTimeOffset.UtcNow.ToString(CultureInfo.InvariantCulture));
            result.Add("Last Updated", File.GetCreationTime(typeof(WhoAmIHealthCheck).Assembly.Location).ToString(CultureInfo.InvariantCulture));
            result.Add("Host", context.Request.Host);
            result.Add("Schema", context.Request.Scheme);
        }
    }
}