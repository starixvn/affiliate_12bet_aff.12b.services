using Aff12B.CronJobs.Core.Domain;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Infrastructure.Constants;
using Aff12B.CronJobs.Infrastructure.Exceptions;
using Aff12B.CronJobs.Infrastructure.SpParams;
using Aff12B.CronJobs.Infrastructure.SpParams.Commands;
using Fanex.Data.Repository;

namespace Aff12B.CronJobs.Infrastructure.Repository
{
    public class AffMemberImportRepository(IDynamicRepository dynamicRepository) : IAffMemberRepository
    {
        public async Task<DateTime> GetLastedMemberImportedTimeAsync(ISettingInfo setting)
            => await dynamicRepository.GetAsync<DateTime>((MemberImportedTimeInfo)setting);

        public async Task<string> GetSiteNameAsync(ISettingInfo settingInfo)
            => await dynamicRepository.GetAsync<string>((SiteNameInfo)settingInfo);

        public async Task ImportMemberAsync(IAffMemberImportInfo memberInfo)
        {
            var memberImportInfo = (InsertMemberInfoCommand)memberInfo;
            await dynamicRepository.ExecuteAsync(memberImportInfo);
            if (memberImportInfo.ErrCode != 0)
            {
                throw new ImporterException(memberImportInfo.ErrCode.ToString(), memberImportInfo.ErrMsg, memberImportInfo.XmlData);
            }
        }

        public async Task InsertAffMemberTimeTrackerAsync(IInsertAffMemberTimeTracker timeTracker)
        {
            var command = (InsertAffMemberTimeTrackerCommand)timeTracker;
            await dynamicRepository.ExecuteAsync(command);

            if (command.ErrCode != SqlResponseCode.Success)
            {
                throw new ImporterException(command.ErrCode, command.ErrMsg);
            }
        }
    }
}