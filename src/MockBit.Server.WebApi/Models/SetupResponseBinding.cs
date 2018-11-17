using System.Collections.Generic;

namespace MockBit.Server.WebApi.Models
{
    /// <summary>
    /// Setup response
    /// </summary>
    public class SetupResponseBinding
    {
        /// <summary>
        /// Http-code. 200, 204, 404, ect.
        /// </summary>
        public int HttpCode { get; set; }

        /// <summary>
        /// Key-value collection respose headers
        /// </summary>
        public IDictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Response body
        /// </summary>
        public string Body { get; set; }
    }
}
