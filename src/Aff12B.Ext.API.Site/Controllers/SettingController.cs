using Aff12B.Common.DataDomain.Models;
using Aff12B.Ext.API.Infastructure.Criteria;
using Fanex.Data.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Aff12B.Ext.API.Site.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SettingController : ControllerBase
    {
        private readonly IDynamicRepository _dynamicRepository;

        public SettingController(IDynamicRepository dynamicRepository)
        {
            _dynamicRepository = dynamicRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<ServerInfoModel>> GetServerInfo()
            => await _dynamicRepository.FetchAsync<ServerInfoModel>(new GetServerInfoCriteria());

        [HttpGet]
        public async Task<IEnumerable<TpGameTypeModel>> GetTpGameType()
            => await _dynamicRepository.FetchAsync<TpGameTypeModel>(new GetTpGameTypeCriteria());
    }
}
