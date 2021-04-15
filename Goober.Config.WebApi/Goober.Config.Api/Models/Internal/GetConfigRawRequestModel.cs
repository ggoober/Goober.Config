namespace Goober.Config.Api.Models.Internal
{
    internal class GetConfigRawRequestModel
    {
        public string Environment { get; set; }

        public string Application { get; set; }

        public string Key { get; set; }

        public string ParentKey { get; set; }
    }
}
