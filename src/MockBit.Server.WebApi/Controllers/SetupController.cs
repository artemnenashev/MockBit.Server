using Microsoft.AspNetCore.Mvc;
using MockBit.Server.WebApi.Models;
using System.Threading.Tasks;

namespace MockBit.Server.WebApi.Controllers
{
    [ApiController]
    public class SetupController : ControllerBase
    {
        [HttpPost("/setup")]
        public async Task<IActionResult> SetupMethod([FromBody] SetupMethodBinding binding)
        {
            return NoContent();
        }
    }
}
