using Goober.Config.Api.Enums;

namespace Goober.Config.Api.Models.Internal
{
    internal class GetPathChildsAndSectionsKeysRequest
    {
        public string ParentKey { get; set; }

        public string Environment { get; set; }

        public string Application { get; set; }

        public GetConfigRowSelectConditionTypeEnum? SelectConditionType { get; set; }
    }
}
