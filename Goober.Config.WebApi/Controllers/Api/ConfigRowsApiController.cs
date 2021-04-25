using Goober.Core.Extensions;
using Goober.Config.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goober.Config.WebApi.Controllers.Api
{
    [ApiController]
    public class ConfigRowsApiController : ControllerBase
    {
        class ConfigRow
        {
            public string Environment { get; set; }

            public string Application { get; set; }

            public string Key { get; set; }

            public string ParentKey { get; set; }

            public string Value { get; set; }
        }

        private static List<ConfigRow> ConfigRaws = new List<ConfigRow> 
        {
            new ConfigRow { Environment = "Production", Application = "Goober.WebApi.Example", Key = "Span", Value="Span", ParentKey = null },
            new ConfigRow { Environment = "Production", Application = "Goober.WebApi.Example", Key = "A", ParentKey = null, Value="A" },
            new ConfigRow { Environment = "Production", Application = "Goober.WebApi.Example", Key = "Text", ParentKey = "Doc:Body:Div", Value="Doc:Body:Div:Text" },
            new ConfigRow { Environment = "Production", Application = "Goober.WebApi.Example", Key = "Select", ParentKey = "Doc:Body:Div", Value="Doc:Body:Div:Select" },
            new ConfigRow { Environment = "Production", Application = "Goober.WebApi.Example", Key = "Text", ParentKey = "Doc:Body:Divs:0", Value="Doc:Body:Divs:0:Text" },
            new ConfigRow { Environment = "Production", Application = "Goober.WebApi.Example", Key = "Select", ParentKey = "Doc:Body:Divs:0", Value="Doc:Body:Divs:0:Select" },
            new ConfigRow { Environment = "Production", Application = "Goober.WebApi.Example", Key = "Text", ParentKey = "Doc:Body:Divs:1", Value="Doc:Body:Divs:1:Text" },
            new ConfigRow { Environment = "Production", Application = "Goober.WebApi.Example", Key = "Select", ParentKey = "Doc:Body:Divs:1", Value="Doc:Body:Divs:1:Select" }

        };

        private readonly ILogger<ConfigRowsApiController> _logger;

        public ConfigRowsApiController(ILogger<ConfigRowsApiController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("api/get-config-row")]
        public async Task<GetConfigRowResponseModel> GetConfigRowAsync([FromBody] GetConfigRowRequestModel request)
        {
            request.RequiredArgumentNotNull(nameof(request));
            request.RequiredArgumentNotNull(() => request.Environment);
            request.RequiredArgumentNotNull(() => request.Key);

            var ret = ConfigRaws.FirstOrDefault(x => x.Environment == request.Environment
                        && x.Application == request.Application
                        && x.Key == request.Key
                        && x.ParentKey == request.ParentKey);

            if (ret == null)
            {
                return null;
            }

            return new GetConfigRowResponseModel {  ResultType = Enums.GetConfigResultTypeEnum.Exact, Value = ret.Value };
        }

        [HttpPost]
        [Route("api/get-childs-keys-and-sections")]
        public async Task<GetPathChildsAndSectionsKeysResponse> GetPathChildsAndSectionsKeysAsync([FromBody] GetPathChildsAndSectionsKeysRequest request)
        {
            request.RequiredArgumentNotNull(nameof(request));
            request.RequiredArgumentNotNull(()=> request.Environment);
            request.RequiredArgumentNotNull(()=> request.ParentKey);


            var raws = ConfigRaws.Where(x=>x.Environment == request.Environment
                                && x.Application == request.Application
                                && x.ParentKey != null
                                && x.ParentKey.StartsWith(request.ParentKey))
                            .ToList();

            var childs = raws.Where(x => x.ParentKey == request.ParentKey).ToList();

            var ret = new GetPathChildsAndSectionsKeysResponse
            {
                Keys = childs.Select(x => x.Key).ToList()
            };

            var sectionsParentKeys = raws.Where(x => x.ParentKey != request.ParentKey).Select(x => x.ParentKey).Distinct().ToList();
            if (sectionsParentKeys.Any() && string.IsNullOrEmpty(request.ParentKey) == false)
            {
                var parentSections = request.ParentKey.Split(":", System.StringSplitOptions.RemoveEmptyEntries);
                var parentSectionsCount = parentSections.Length;
                foreach (var iSectionsParentKey in sectionsParentKeys)
                {
                    var sections = iSectionsParentKey.Split(":",System.StringSplitOptions.RemoveEmptyEntries);
                    if (sections.Length > 1 && sections.Length <= parentSectionsCount)
                    {
                        continue;
                    }

                    //get next from parent last section
                    var sectionKey = sections[parentSectionsCount];
                    if (ret.Sections.All(x => x != sectionKey))
                    {
                        ret.Sections.Add(sectionKey);
                    }
                }
            }

            return ret;
        }
    }
}
