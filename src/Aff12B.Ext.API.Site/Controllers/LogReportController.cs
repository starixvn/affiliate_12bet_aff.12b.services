using Aff12B.Common.DataDomain.Models;
using Aff12B.Ext.API.Infastructure.Criteria;
using Fanex.Data.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aff12B.Ext.API.Site.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class LogReportController : ControllerBase
    {
        public IDynamicRepository _repo { get; }

        public LogReportController(IDynamicRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IEnumerable<RebateMLogModel>> GetRebateMLogs(DateTime fromDate, DateTime toDate)
            => await _repo.FetchAsync<RebateMLogModel>(new GetRebateMLogCriteria(fromDate, toDate));

    }
}
