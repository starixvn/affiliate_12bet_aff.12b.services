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
    public class TagController : ControllerBase
    {
        private readonly IDynamicRepository dynamicRepository;

        public TagController(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<TagModel>> GetTags(DateTime updatedTime, int batchCount = 2000)
            => await dynamicRepository.FetchAsync<TagModel>(new GetTagCriteria(updatedTime, batchCount));
    }
}
