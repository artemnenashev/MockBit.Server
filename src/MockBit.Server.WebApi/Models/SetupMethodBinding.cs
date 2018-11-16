namespace MockBit.Server.WebApi.Models
{
    public class SetupMethodBinding
    {
        public string Method { get; set; }

        public string Route { get; set; }

        public SetupResponseBinding Response { get; set; }
    }
}
