using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Constants;
using Microsoft.Extensions.Options;

namespace Aff12B.CronJobs.Infrastructure.Jobs.Deposit
{
    public class FetchDepositInfoDataTask : BaseDepositInfoDataTask
    {
        public FetchDepositInfoDataTask(IThirdPartyImportRepository thirdPartyImportRepository
            , ILicenseeApi licenseeApi
            , ITrackerService trackerService
            , IOptionsSnapshot<FetchServiceSetting> setting)
            : base(thirdPartyImportRepository, licenseeApi, trackerService, setting.Get(ServiceConfigName.PULL_DEPOSIT_INFO_SETTING))
        { }
    }
}
