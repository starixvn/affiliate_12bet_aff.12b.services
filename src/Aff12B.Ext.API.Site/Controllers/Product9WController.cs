using Aff12B.Common.DataDomain.Models;
using Aff12B.Common.DataDomain.Params;
using Aff12B.Ext.API.Infastructure.Criteria;
using Fanex.Data.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aff12B.Ext.API.Site.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class Product9WController : ControllerBase
    {

        private readonly IDynamicRepository _dynamicRepository;

        public Product9WController(IDynamicRepository dynamicRepository)
        {
            _dynamicRepository = dynamicRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<TpReport9WModel>> GetTpReport([FromQuery] BaseParam param)
            => await _dynamicRepository.FetchAsync<TpReport9WModel>(new GetTpReport9WCriteria
            {
                SQ = param.SQ,
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                PageIndex = param.PageIndex,
                PageSize = param.PageSize
            });

        [HttpGet]
        public async Task<IEnumerable<TpWager9WModel>> GetTpWager([FromQuery] BaseParam param)
            => await _dynamicRepository.FetchAsync<TpWager9WModel>(new GetTpWager9WCriteria
            {
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                PageIndex = param.PageIndex,
                PageSize = param.PageSize
            });
    }
}
