using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Aff12B.Ext.API.Site.Controllers
{
    [Route("api/[controller]/[action]")]
    [AllowAnonymous]
    public class ErrorController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return new JsonResult(new { Message = "Error" });
        }

        [HttpGet]
        public IActionResult UnAuthorized()
        {
            return new JsonResult(new { Message = "UnAuthorized" });
        }

        [HttpGet]
        public IActionResult NotWhiteList()
        {
            return new JsonResult(new { Message = "NotWhiteList" });
        }
    }
}
