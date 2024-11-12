using Aff12B.Ext.API.Infastructure.Criteria;
using Aff12B.Common.DataDomain.Models;
using Fanex.Data.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Aff12B.Ext.API.Site.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class BetReportController : ControllerBase
    {
        private readonly IDynamicRepository dynamicRepository;

        public BetReportController(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<TpBetRecordModel>> GetBetRecord(DateTime fromDate, DateTime toDate, int pageIndex, int pageSize = 2000)
        {
            var betlist = await dynamicRepository.FetchAsync<TpBetRecordModel>(new GetTpBetRecordCriteria(fromDate, toDate, pageIndex, pageSize));
            return betlist;
        }

        [HttpGet]
        public async Task<IEnumerable<DepositInfoModel>> GetDepositInfo(DateTime fromDate, DateTime toDate, int pageIndex, int pageSize = 2000)
            => await dynamicRepository.FetchAsync<DepositInfoModel>(new GetDepositInfoCriteria(fromDate, toDate, pageIndex, pageSize));

        [HttpGet]
        public async Task<IEnumerable<BonusModel>> GetBonusInfo(int id, int batchCount = 2000)
            => await dynamicRepository.FetchAsync<BonusModel>(new GetBonusInfoCriteria(id, batchCount));

        [HttpGet]
        public async Task<IEnumerable<BonusModel>> GetBonusInfoByDate(DateTime calledTime, int pageIndex, int pageSize = 2000)
            => await dynamicRepository.FetchAsync<BonusModel>(new GetBonusInfoByDateCriteria(calledTime, pageIndex, pageSize));

        [HttpGet]
        public async Task<IEnumerable<WithdrawalInfoModel>> GetWithdrawalInfo(DateTime updateTime, int batchCount = 2000)
            => await dynamicRepository.FetchAsync<WithdrawalInfoModel>(new GetWithdrawalInfoCriteria(updateTime, batchCount));

    }
}
