namespace MockBit.Server.WebApi.Models
{
    /// <summary>
    /// Setup behavior for http method and route
    /// </summary>
    public class SetupMethodBinding
    {
        /// <summary>
        /// Http method: GET, POST, PUT, etc.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Route string. Excample: /entities/{id}
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// Response which must be received after execute http method by route
        /// </summary>
        public SetupResponseBinding Response { get; set; }
    }
}
