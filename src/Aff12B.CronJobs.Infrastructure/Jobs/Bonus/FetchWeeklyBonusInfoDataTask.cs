using Aff12B.Common.DataDomain.Models;
using Aff12B.CronJobs.Core.Domain.DTOs;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Constants;
using Aff12B.CronJobs.Infrastructure.Helpers;
using Aff12B.CronJobs.Infrastructure.Jobs.Base;
using Hangfire;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Aff12B.CronJobs.Infrastructure.Jobs.Bonus
{
    public class FetchWeeklyBonusInfoDataTask : FetchDataForMultipleRepositoryTaskBase<BonusModel>
    {
        private const int SUBSTRACT_DATE = -39;

        private readonly ILicenseeApi _licenseeApi;
        private readonly ITrackerService _trackerService;

        public FetchWeeklyBonusInfoDataTask(ILicenseeApi licenseeApi,
            IThirdPartyImportRepository _thirdPartyRepo,
            ITrackerService timeTrackerService,
            IOptionsSnapshot<FetchServiceSetting> configuration)
            : base(_thirdPartyRepo, configuration.Get(ServiceConfigName.PULL_WEEKLY_BONUS_INFO_SETTING))
        {
            _licenseeApi = licenseeApi ?? throw new ArgumentNullException(nameof(licenseeApi));
            _trackerService = timeTrackerService;
        }

        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        public override async Task FetchDataAsync()
        {
            var date = GetLastMonthDate();
            var now = DateTime.UtcNow;
            while (date <= now)
            {
                await PullAndImportData(date);
                date = date.AddDays(1);
            }
        }

        private async Task PullAndImportData(DateTime date)
        {
            var isRunFirstTime = true;
            var listData = Enumerable.Empty<BonusModel>();
            var pageIndex = 0;
            while (isRunFirstTime || listData.Count() == 2000)
            {
                isRunFirstTime = false;
                listData = await _licenseeApi.GetBonusInfoByDate(date, pageIndex, SettingConstant.MAX_ROW_COUNT);
                await ImportDataToMultipleRepoAsync(listData);
                var trackerItem = new { Date = date, PageIndex = pageIndex };
                await _trackerService.SaveTrackerAsync(TableName,
                    new TrackerInfo
                    {
                        CalledTime = DateTime.Now,
                        KeyValue = JsonConvert.SerializeObject(trackerItem)
                    });
                pageIndex++;
            }
        }

        private DateTime GetLastMonthDate()
            => DateTime.Now.AddDays(SUBSTRACT_DATE).Date;


        protected override string BuildXml(IEnumerable<BonusModel> data)
             => XmlHelper.BuildXml(data, "root", "row");

        protected override IEnumerable<string> GetSupportedRepoPrefix()
            => Settings.SupportedPrefix;
    }
}
