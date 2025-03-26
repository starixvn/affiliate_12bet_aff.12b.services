using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Constants;
using Aff12B.CronJobs.Infrastructure.Jobs.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aff12B.CronJobs.Infrastructure.Jobs.AffCustomer
{
    public sealed class FetchRealAffCustomerDataTask : BaseAffCustomerDataTask, IFetchDataTask
    {
        protected override bool IsNeedRealData => true;

        public FetchRealAffCustomerDataTask(ILicenseeApi licenseeApi
            , IAffMemberRepository memberImportRepository
            , ILogger<FetchAffCustomerDataTask> logger
            , IOptions<LicenseeSiteSetting> config
            , IOptionsSnapshot<FetchServiceSetting> configuration)
            : base(licenseeApi, memberImportRepository, logger, config.Value, configuration.Get(ServiceConfigName.PULL_REAL_AFF_CUSTOMER_SETTING))
        {
        }
    }
}
