using Aff12B.Common.DataDomain.Params;
using Aff12B.CronJobs.Infrastructure.Apis;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;


namespace Aff12B.CronJobs.Infrastructure.Handlers
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private const string TOKEN_MEMORY_CACHE = "TOKEN_MEMORY_12B_V2_CRONJOBS_CAHCE";

        private readonly ITokenApi _tokenApi;
        private readonly IMemoryCache _memoryCache;
        private readonly TokenRequest _token;

        public AuthHeaderHandler(ITokenApi tokenApi, IMemoryCache memoryCache, IOptions<TokenRequest> token)
        {
            _tokenApi = tokenApi ?? throw new ArgumentNullException(nameof(tokenApi));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _token = token.Value ?? throw new ArgumentNullException(nameof(token));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _memoryCache.GetOrCreateAsync(TOKEN_MEMORY_CACHE, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_token.CacheTimeOut);
                var tokenInfo = await _tokenApi.GetTokenAsync(_token);
                return tokenInfo.AccessToken;
            });

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
