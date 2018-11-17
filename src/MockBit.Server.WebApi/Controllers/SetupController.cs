using Microsoft.AspNetCore.Mvc;
using MockBit.Server.WebApi.Models;
using System.Threading;
using System.Threading.Tasks;

namespace MockBit.Server.WebApi.Controllers
{
    [ApiController]
    public class SetupController : ControllerBase
    {
        /// <summary>
        /// Setup behavior for http method and route
        /// </summary>
        /// <param name="binding">Method, route and response</param>
        /// <response code="204">Success</response>
        /// <response code="400">Bad request</response>
        [HttpPost("/setup")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> SetupMethod([FromBody] SetupMethodBinding binding, CancellationToken cancellationToken)
        {
            return NoContent();
        }

        /// <summary>
        /// Clear all behaviors and reset to primary state
        /// </summary>
        /// <response code="204">Success</response>
        [HttpPost("/reset")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> Reset(CancellationToken cancellationToken)
        {
            return NoContent();
        }
    }
}
