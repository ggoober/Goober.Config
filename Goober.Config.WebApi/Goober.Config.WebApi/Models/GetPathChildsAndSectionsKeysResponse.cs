using System.Collections.Generic;

namespace Goober.Config.WebApi.Models
{
    public class GetPathChildsAndSectionsKeysResponse
    {
        public List<string> Keys { get; set; } = new List<string>();

        public List<string> Sections { get; set; } = new List<string>();
    }
}
