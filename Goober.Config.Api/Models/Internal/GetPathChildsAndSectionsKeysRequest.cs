namespace Goober.Config.Api.Models.Internal
{
    internal class GetPathChildsAndSectionsKeysRequest
    {
        public string ParentKey { get; set; }

        public string Environment { get; set; }

        public string Application { get; set; }
    }
}
