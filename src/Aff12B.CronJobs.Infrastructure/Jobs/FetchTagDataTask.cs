using Aff12B.Common.DataDomain.Models;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Constants;
using Aff12B.CronJobs.Infrastructure.Jobs.Base;
using Microsoft.Extensions.Options;

namespace Aff12B.CronJobs.Infrastructure.Jobs
{
    public class FetchTagDataTask(
        IThirdPartyImportRepository thirdPartyImportRepository,
        ITrackerService timeTrackerService,
        ILicenseeApi licenseeApi,
        IOptionsSnapshot<FetchServiceSetting> configuration) : FetchDataByDateTimeTrackerForMultiRepoTaskBase<TagModel>
            (thirdPartyImportRepository, configuration.Get(ServiceConfigName.PULL_TAG_SETTING), timeTrackerService)
    {
        protected override Task<IEnumerable<TagModel>> GetDataAsync(string trackValue, int batchCount)
            => licenseeApi.GetTagsAsync(Convert.ToDateTime(trackValue), batchCount);


    }
}
