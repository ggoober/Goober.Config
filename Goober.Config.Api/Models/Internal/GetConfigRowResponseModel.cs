using Goober.Config.Api.Enums;

namespace Goober.Config.Api.Models.Internal
{
    internal class GetConfigRowResponseModel
    {
        public GetConfigRowSelectConditionTypeEnum SelectedConditionType { get; set; }

        public string Value { get; set; }
    }
}
