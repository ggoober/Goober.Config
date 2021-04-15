namespace Goober.Config.WebApi.Models
{
    public class GetPathChildsAndSectionsKeysRequest
    {
        public string ParentKey { get; set; }

        public string Environment { get; set; }

        public string Application { get; set; }
    }
}
