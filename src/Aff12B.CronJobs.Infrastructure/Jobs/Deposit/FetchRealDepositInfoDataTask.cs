using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Constants;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aff12B.CronJobs.Infrastructure.Jobs.Deposit
{
    public class FetchRealDepositInfoDataTask : BaseDepositInfoDataTask
    {
        protected override bool IsNeedRealData => true;

        public FetchRealDepositInfoDataTask(IThirdPartyImportRepository thirdPartyImportRepository
            , ILicenseeApi licenseeApi
            , ILogger<FetchDepositInfoDataTask> logger
            , ITrackerService trackerService
            , IOptionsSnapshot<FetchServiceSetting> setting)
            : base(thirdPartyImportRepository, licenseeApi, logger, trackerService, setting.Get(ServiceConfigName.PULL_REAL_DEPOSIT_INFO_SETTING))
        { }
    }
}
