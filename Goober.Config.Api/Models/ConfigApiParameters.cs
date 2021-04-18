namespace Goober.Config.Api.Models
{
    public class ConfigApiParameters
    {
        public int? CacheRefreshTimeInMinutes { get; set; }

        public int? CacheExpirationTimeInMinutes { get; set; }
    }
}
