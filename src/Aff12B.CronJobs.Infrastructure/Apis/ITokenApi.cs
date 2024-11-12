using Aff12B.Common.DataDomain.Api;
using Aff12B.Common.DataDomain.Params;
using Refit;

namespace Aff12B.CronJobs.Infrastructure.Apis
{
    public interface ITokenApi
    {
        [Post("/Auth")]
        Task<TokenInfo> GetTokenAsync([Body] TokenRequest param);
    }
}
