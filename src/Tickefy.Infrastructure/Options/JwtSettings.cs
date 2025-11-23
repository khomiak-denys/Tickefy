namespace Tickefy.Infrastructure.Options
{
    public class JwtSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int TokenValidityMins { get; set; }
    }
}
