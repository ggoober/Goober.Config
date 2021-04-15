namespace Goober.Config.WebApi.Models
{
    public class GetConfigRawRequestModel
    {
        public string Environment { get; set; }

        public string Application { get; set; }

        public string Key { get; set; }

        public string ParentKey { get; set; }
    }
}
