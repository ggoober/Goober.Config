using Goober.Config.WebApi.Models;
using Goober.Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goober.Config.WebApi.Controllers.Api
{
    [ApiController]
    public class ConfigRawsApiController : ControllerBase
    {
        class ConfigRaw
        {
            public string Environment { get; set; }

            public string Application { get; set; }

            public string Key { get; set; }

            public string ParentKey { get; set; }

            public string Value { get; set; }
        }

        private static List<ConfigRaw> Configs = new List<ConfigRaw> 
        {
            new ConfigRaw { Environment = "Production", Application = "Goober.WebApi.Example", Key = "span", Value="span", ParentKey = null },
            new ConfigRaw { Environment = "Production", Application = "Goober.WebApi.Example", Key = "a", ParentKey = null, Value="a" },
            new ConfigRaw { Environment = "Production", Application = "Goober.WebApi.Example", Key = "div", ParentKey = "doc:body:div", Value="doc:body:div:div" },
            new ConfigRaw { Environment = "Production", Application = "Goober.WebApi.Example", Key = "select", ParentKey = "doc:body:div", Value="doc:body:div:select" },
            new ConfigRaw { Environment = "Production", Application = "Goober.WebApi.Example", Key = "div", ParentKey = "doc:body:div:0", Value="doc:body:div:0:div" },
            new ConfigRaw { Environment = "Production", Application = "Goober.WebApi.Example", Key = "select", ParentKey = "doc:body:div:0", Value="doc:body:div:0:select" },
            new ConfigRaw { Environment = "Production", Application = "Goober.WebApi.Example", Key = "div", ParentKey = "doc:body:div:0:div", Value="doc:body:div:0:div:div" },
            new ConfigRaw { Environment = "Production", Application = "Goober.WebApi.Example", Key = "select", ParentKey = "doc:body:div:0:div", Value="doc:body:div:0:div:select" }

        };

        private readonly ILogger<ConfigRawsApiController> _logger;

        public ConfigRawsApiController(ILogger<ConfigRawsApiController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("api/get-config-raw")]
        public async Task<GetConfigRawResponseModel> GetConfigRawAsync([FromBody] GetConfigRawRequestModel request)
        {
            request.RequiredArgumentNotNull(nameof(request));
            request.RequiredArgumentNotNull(() => request.Environment);
            request.RequiredArgumentNotNull(() => request.Key);

            var ret = Configs.FirstOrDefault(x => x.Environment == request.Environment
                        && x.Application == request.Application
                        && x.Key == request.Key
                        && x.ParentKey == request.ParentKey);

            if (ret == null)
            {
                return null;
            }

            return new GetConfigRawResponseModel {  ResultType = Enums.GetConfigResultTypeEnum.Exact, Value = ret.Value };
        }

        [HttpPost]
        [Route("api/get-childs-keys-and-sections")]
        public async Task<GetPathChildsAndSectionsKeysResponse> GetPathChildsAndSectionsKeysAsync([FromBody] GetPathChildsAndSectionsKeysRequest request)
        {
            request.RequiredArgumentNotNull(nameof(request));
            request.RequiredArgumentNotNull(()=> request.Environment);
            request.RequiredArgumentNotNull(()=> request.ParentKey);


            var raws = Configs.Where(x=>x.Environment == request.Environment
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
