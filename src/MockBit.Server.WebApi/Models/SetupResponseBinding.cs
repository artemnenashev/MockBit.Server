using System.Collections.Generic;

namespace MockBit.Server.WebApi.Models
{
    public class SetupResponseBinding
    {
        public int HttpCode { get; set; }

        public IDictionary<string, string> Headers { get; set; }

        public string Body { get; set; }
    }
}
