using Aff12B.Common.DataDomain.Models;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Constants;
using Aff12B.CronJobs.Infrastructure.Helpers;
using Aff12B.CronJobs.Infrastructure.Jobs.Base;
using Hangfire;
using Microsoft.Extensions.Options;

namespace Aff12B.CronJobs.Infrastructure.Jobs
{
    public class FetchServerInfoDataTask : FetchDataForMultipleRepositoryTaskBase<ServerInfoModel>
    {
        private readonly ILicenseeApi _licenseeApi;

        public FetchServerInfoDataTask(IThirdPartyImportRepository thirdPartyImportRepository
            , ILicenseeApi licenseeApi
            , IOptionsSnapshot<FetchServiceSetting> setting)
            : base(thirdPartyImportRepository, setting.Get(ServiceConfigName.PULL_SERVER_INFO_SETTING))
        {
            _licenseeApi = licenseeApi ?? throw new ArgumentNullException(nameof(licenseeApi));
        }

        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        public async override Task FetchDataAsync()
        {
            var data = await _licenseeApi.GetServerInfoAsync();
            await ImportDataToMultipleRepoAsync(data);
        }

        protected override string BuildXml(IEnumerable<ServerInfoModel> data)
            => XmlHelper.BuildXml(data, "root", "row");

        protected override IEnumerable<string> GetSupportedRepoPrefix()
            => Settings.SupportedPrefix;
    }
}
