using Goober.Config.Api.Enums;

namespace Goober.Config.Api.Models.Internal
{
    internal class GetConfigRawResponseModel
    {
        public GetConfigResultTypeEnum ResultType { get; set; }

        public string Value { get; set; }
    }
}
