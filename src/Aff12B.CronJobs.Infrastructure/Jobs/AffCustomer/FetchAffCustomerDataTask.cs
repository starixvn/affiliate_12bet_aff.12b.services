using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Constants;
using Aff12B.CronJobs.Infrastructure.Jobs.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aff12B.CronJobs.Infrastructure.Jobs.AffCustomer
{
    public interface IFetchAffCustomerDataTask : IFetchDataTask
    {
        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        Task FetchDataByDateRangeTask();
    }

    public sealed class FetchAffCustomerDataTask : BaseAffCustomerDataTask, IFetchAffCustomerDataTask
    {
        private const int DATE_RANGE = 1440;

        public FetchAffCustomerDataTask(ILicenseeApi licenseeApi
            , IAffMemberRepository memberImportRepository
            , ILogger<FetchAffCustomerDataTask> logger
            , IOptions<LicenseeSiteSetting> config
            , IOptionsSnapshot<FetchServiceSetting> configuration)
        : base(licenseeApi, memberImportRepository, logger, config.Value, configuration.Get(ServiceConfigName.PULL_AFF_CUSTOMER_SETTING))
        { }

        public async Task FetchDataByDateRangeTask()
        {
            if (SupportedSites == null
                || !SupportedSites.Any())
            {
                return;
            }

            var maxTimeRange = TimeSpan.FromMinutes(DATE_RANGE);

            await FetchDataAsync(maxTimeRange);
        }
    }
}