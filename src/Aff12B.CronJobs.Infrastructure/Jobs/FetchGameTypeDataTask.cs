using Aff12B.Common.DataDomain.Models;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Constants;
using Aff12B.CronJobs.Infrastructure.Helpers;
using Aff12B.CronJobs.Infrastructure.Jobs.Base;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aff12B.CronJobs.Infrastructure.Jobs
{
    public class FetchGameTypeDataTask : FetchDataForMultipleRepositoryTaskBase<TpGameTypeModel>
    {
        private readonly ILicenseeApi _licenseeApi;
        private readonly ILogger _logger;

        public FetchGameTypeDataTask(IThirdPartyImportRepository thirdPartyImportRepository
            , ILicenseeApi licenseeApi
            , ILogger<FetchGameTypeDataTask> logger
            , IOptionsSnapshot<FetchServiceSetting> setting)
            : base(thirdPartyImportRepository, setting.Get(ServiceConfigName.PULL_GAME_TYPE_SETTING))
        {
            _licenseeApi = licenseeApi ?? throw new ArgumentNullException(nameof(licenseeApi));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        public async override Task FetchDataAsync()
        {
            var data = await _licenseeApi.GetGameTypeAsync();
            await ImportDataToMultipleRepoAsync(data);
        }

        protected override string BuildXml(IEnumerable<TpGameTypeModel> data)
            => XmlHelper.BuildXml(data, "root", "row");

        protected override IEnumerable<string> GetSupportedRepoPrefix()
            => Settings.SupportedPrefix;
    }
}
