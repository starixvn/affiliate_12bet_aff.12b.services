using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aff12B.Ext.API.Site.Controllers
{
    [Route("api/[controller]/[action]")]
    [AllowAnonymous]
    [ApiController]
    public class TestController : Controller
    {
        private readonly ILogger _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogInformation("TestController.Index");
            return new JsonResult(new { Message = "Test" });
        }
    }
}
