using Goober.Config.Api.Enums;

namespace Goober.Config.Api.Models.Internal
{
    internal class GetConfigRowRequestModel
    {
        public string Environment { get; set; }

        public string Application { get; set; }

        public string Key { get; set; }

        public string ParentKey { get; set; }

        public GetConfigRowSelectConditionTypeEnum? SelectCondiitionType { get; set; }
    }
}
