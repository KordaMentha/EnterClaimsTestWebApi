using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EnterClaimsTestWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnterClaimsTestController : ControllerBase
    {
        private readonly DataService _dataverseService;
        public EnterClaimsTestController(DataService dataverseService)
        {
            _dataverseService = dataverseService;
        }

        [HttpGet("GetBSBs")]
        public async Task<IActionResult> GetAllBSBs()
        {
            var result = await _dataverseService.GetBSBAsync();
            return Ok(result);
        }

    }
}
