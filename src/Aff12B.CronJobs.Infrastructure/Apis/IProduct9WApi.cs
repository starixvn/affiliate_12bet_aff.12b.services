using Aff12B.Common.DataDomain.Models;
using Aff12B.Common.DataDomain.Params;
using Refit;

namespace Aff12B.CronJobs.Infrastructure.Apis
{
    public interface IProduct9WApi
    {
        [Get("/Product9W/GetTpReport")]
        Task<IEnumerable<TpReport9WModel>> GetTpReportAsync(BatchCountParam param);

        [Get("/Product9W/GetTpWager")]
        Task<IEnumerable<TpWager9WModel>> GetTpWagerAsync(BatchCountParam param);
    }
}
