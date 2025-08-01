using Aff12B.CronJobs.Core.Domain;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Infrastructure.Exceptions;
using Aff12B.CronJobs.Infrastructure.SpParams.Commands;
using Fanex.Data.Repository;

namespace Aff12B.CronJobs.Infrastructure.Repository
{
    public class ThirdPartyImportRepository : IThirdPartyImportRepository
    {
        private readonly IDynamicRepository _dynamicRepository;

        public ThirdPartyImportRepository(IDynamicRepository dynamicRepository)
        {
            _dynamicRepository = dynamicRepository;
        }

        public async Task ImportRecordAsync(IPushDataRecord betRecord)
        {
            var command = (InsertThirdPartyRecodInfoCommand)betRecord;
            await _dynamicRepository.ExecuteAsync(command);
            if (betRecord.ErrCode != 0)
            {
                throw new ImporterException(command.ErrCode.ToString(), command.ErrMsg, command.XmlData);
            }
        }
    }
}
