using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EnterClaimsTestWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("SimpleTest")]
        public IActionResult SimpleTest()
        {
            string[] testCodes = new string[] { "123456", "654321", "111222", "333444", "999999", "1098765" };
            return Ok(testCodes);
        }

    }

}
