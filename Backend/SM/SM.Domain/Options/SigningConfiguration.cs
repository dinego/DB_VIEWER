namespace SM.Domain.Options
{
    public class SigningConfiguration
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SimulatedUser { get; set; }
        public string Products { get; set; }
    }
}
