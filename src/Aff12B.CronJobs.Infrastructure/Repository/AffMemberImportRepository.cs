using Aff12B.CronJobs.Core.Domain;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Infrastructure.Exceptions;
using Aff12B.CronJobs.Infrastructure.SpParams;
using Aff12B.CronJobs.Infrastructure.SpParams.Commands;
using Fanex.Data.Repository;

namespace Aff12B.CronJobs.Infrastructure.Repository
{
    public class AffMemberImportRepository : IAffMemberRepository
    {
        private readonly IDynamicRepository _dynamicRepository;

        public AffMemberImportRepository(IDynamicRepository dynamicRepository)
        {
            _dynamicRepository = dynamicRepository;
        }

        public async Task<DateTime> GetLastedMemberImportedTimeAsync(ISettingInfo setting)
            => await _dynamicRepository.GetAsync<DateTime>((MemberImportedTimeInfo)setting);

        public async Task<string> GetSiteNameAsync(ISettingInfo settingInfo)
            => await _dynamicRepository.GetAsync<string>((SiteNameInfo)settingInfo);

        public async Task ImportMemberAsync(IAffMemberImportInfo memberInfo)
        {
            var memberImportInfo = (InsertMemberInfoCommand)memberInfo;
            await _dynamicRepository.ExecuteAsync(memberImportInfo);
            if (memberImportInfo.ErrCode != 0)
            {
                throw new ImporterException(memberImportInfo.ErrCode.ToString(), memberImportInfo.ErrMsg, memberImportInfo.XmlData);
            }
        }
    }
}