using Goober.Config.WebApi.Enums;

namespace Goober.Config.WebApi.Models
{
    public class GetConfigRawResponseModel
    {
        public GetConfigResultTypeEnum ResultType { get; set; }

        public string Value { get; set; }
    }
}
