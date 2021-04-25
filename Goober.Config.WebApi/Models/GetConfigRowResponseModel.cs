using Goober.Config.WebApi.Enums;

namespace Goober.Config.WebApi.Models
{
    public class GetConfigRowResponseModel
    {
        public GetConfigRowSelectConditionTypeEnum SelectedConditionType { get; set; }

        public string Value { get; set; }
    }
}
