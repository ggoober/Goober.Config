namespace Goober.Config.Api.Models
{
    public class HttpConfigParameters
    {
        public string Environment { get; set; }

        public string ApplicationName { get; set; }

        public string ApiSchemeAndHost { get; set; }

        public ConfigApiParameters ConfigApiParameters { get; set; }
    }
}
