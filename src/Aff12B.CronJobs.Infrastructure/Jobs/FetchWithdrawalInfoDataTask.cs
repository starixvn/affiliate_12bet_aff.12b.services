using Aff12B.Common.DataDomain.Models;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Jobs.Base;
using Microsoft.Extensions.Options;

namespace Aff12B.CronJobs.Infrastructure.Jobs
{
    public class FetchWithdrawalInfoDataTask(
        IThirdPartyImportRepository thirdPartyImportRepository,
        ITrackerService timeTrackerService,
        ILicenseeApi licenseeApi,
        IOptionsSnapshot<FetchServiceSetting> configuration) : FetchDataByDateTimeTrackerForMultiRepoTaskBase<WithdrawalInfoModel>
            (thirdPartyImportRepository, configuration.Get(Constants.ServiceConfigName.PULL_WITHDRAWAL_INFO), timeTrackerService)
    {
        protected override Task<IEnumerable<WithdrawalInfoModel>> GetDataAsync(string trackValue, int batchCount)
            => licenseeApi.GetWithdrawalInfo(Convert.ToDateTime(trackValue), batchCount);


    }
}