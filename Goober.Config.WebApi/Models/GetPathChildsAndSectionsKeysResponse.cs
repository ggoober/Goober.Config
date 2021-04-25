using Goober.Config.WebApi.Enums;
using System.Collections.Generic;

namespace Goober.Config.WebApi.Models
{
    public class GetPathChildsAndSectionsKeysResponse
    {
        public List<string> Keys { get; set; } = new List<string>();

        public GetConfigRowSelectConditionTypeEnum? KeysSelectedConditionType { get; set; }


        public List<string> Sections { get; set; } = new List<string>();

        public GetConfigRowSelectConditionTypeEnum? SectionsSelectedConditionType { get; set; }
    }
}
