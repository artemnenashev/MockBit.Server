using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MockBit.Server.WebApi.Models;
using MockBit.Server.WebApi.Extensions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MockBit.Server.WebApi.Infrastructure;

namespace MockBit.Server.WebApi.Controllers
{
    [ApiController]
    public class SetupController : ControllerBase
    {
        private readonly SynchronizedRouteCollection _routeCollection;

        public SetupController(SynchronizedRouteCollection routeCollection)
        {
            _routeCollection = routeCollection;
        }

        /// <summary>
        /// Setup behavior for http method and route
        /// </summary>
        /// <param name="binding">Method, route and response</param>
        /// <response code="204">Success</response>
        /// <response code="400">Bad request</response>
        [HttpPost("/setup")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> SetupMethod([FromBody] SetupMethodBinding binding,
            [FromServices] IInlineConstraintResolver inlineConstraintResolver,
            CancellationToken cancellationToken)
        {
            _routeCollection.MapVerb(
                inlineConstraintResolver: inlineConstraintResolver,
                verb: binding.Method,
                template: binding.Route,
                handler: async (req, res, rdata) =>
                {
                    res.StatusCode = binding.Response.HttpCode;

                    if (binding.Response.Headers != null)
                    {
                        foreach (var kvp in binding.Response.Headers)
                            res.Headers.Add(kvp.Key, kvp.Value);
                    }

                    await res.Body.WriteAsync(Encoding.UTF8.GetBytes(binding.Response.Body));

                });

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
            _routeCollection.Clear();
            return NoContent();
        }
    }
}
