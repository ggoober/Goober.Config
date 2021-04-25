using Goober.Core.Extensions;
using Goober.Config.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goober.Config.DAL.Repositories;
using Goober.Config.WebApi.Enums;

namespace Goober.Config.WebApi.Controllers.Api
{
    [ApiController]
    public class ConfigRowsApiController : ControllerBase
    {
        private readonly ILogger<ConfigRowsApiController> _logger;
        private readonly IConfigRowRepository _configRowRepository;

        public ConfigRowsApiController(ILogger<ConfigRowsApiController> logger,
            IConfigRowRepository configRowRepository)
        {
            _logger = logger;
            _configRowRepository = configRowRepository;
        }

        [HttpPost]
        [Route("/api/config/get-row")]
        public async Task<GetConfigRowResponseModel> GetConfigRowAsync([FromBody] GetConfigRowRequestModel request)
        {
            request.RequiredArgumentNotNull(nameof(request));
            request.RequiredArgumentNotNull(() => request.Environment);
            request.RequiredArgumentNotNull(() => request.Key);

            var ret = (await _configRowRepository.GetByApplicationAsync(environment: request.Environment,
                        application: request.Application,
                        key: request.Key,
                        parent: request.ParentKey)).FirstOrDefault();

            if (ret != null)
            {
                return new GetConfigRowResponseModel { SelectedConditionType = GetConfigRowSelectConditionTypeEnum.ByApplication, Value = ret.Value };
            }

            ret = (await _configRowRepository.GetExcludeApplicationAsync(environment: request.Environment,
                        key: request.Key,
                        parent: request.ParentKey,
                        anyApplication: false)).FirstOrDefault();

            if (ret != null)
            {
                return new GetConfigRowResponseModel { SelectedConditionType = GetConfigRowSelectConditionTypeEnum.WithoutApplication, Value = ret.Value };
            }

            ret = (await _configRowRepository.GetExcludeApplicationAsync(environment: request.Environment,
                        key: request.Key,
                        parent: request.ParentKey,
                        anyApplication: true)).FirstOrDefault();

            if (ret != null)
            {
                return new GetConfigRowResponseModel { SelectedConditionType = GetConfigRowSelectConditionTypeEnum.IgnoreApplication, Value = ret.Value };
            }

            return null;
        }

        [HttpPost]
        [Route("/api/config/get-childs-keys-and-sections")]
        public async Task<GetPathChildsAndSectionsKeysResponse> GetPathChildsAndSectionsKeysAsync([FromBody] GetPathChildsAndSectionsKeysRequest request)
        {
            request.RequiredArgumentNotNull(nameof(request));
            request.RequiredArgumentNotNull(() => request.Environment);
            request.RequiredArgumentNotNull(() => request.ParentKey);

            var ret = new GetPathChildsAndSectionsKeysResponse();

            var childsResult = await GetChildsAsync(request);
            if (childsResult.HasValue == true)
            {
                ret.Keys = childsResult.Value.Childs;
                ret.KeysSelectedConditionType = childsResult.Value.SelectedConditionType;
            }

            var sectionsResult = await GetSectionsAsync(request);

            if (sectionsResult.HasValue == true 
                && sectionsResult.Value.Sections.Any() == true)
            {
                ret.SectionsSelectedConditionType = sectionsResult.Value.SelectedConditionType;
                ret.Sections = GetSubSections(request.ParentKey, sectionsResult);
            }

            return ret;
        }

        private static List<string> GetSubSections(string parentKey, (List<string> Sections, GetConfigRowSelectConditionTypeEnum SelectedConditionType)? sectionsResult)
        {
            var ret = new List<string>();

            var parentSections = parentKey.Split(":", System.StringSplitOptions.RemoveEmptyEntries);
            var parentSectionsCount = parentSections.Length;
            foreach (var iSection in sectionsResult.Value.Sections)
            {
                var sections = iSection.Split(":", System.StringSplitOptions.RemoveEmptyEntries);
                if (sections.Length > 1 && sections.Length <= parentSectionsCount)
                {
                    continue;
                }

                //get next from parent last section
                var sectionKey = sections[parentSectionsCount];
                if (ret.All(x => x != sectionKey))
                {
                    ret.Add(sectionKey);
                }
            }

            return ret;
        }

        private async Task<(List<string> Childs, GetConfigRowSelectConditionTypeEnum SelectedConditionType)?> GetChildsAsync(GetPathChildsAndSectionsKeysRequest request)
        {
            var childs = await _configRowRepository.GetChildKeysByApplicationAsync(environment: request.Environment,
                                                application: request.Application,
                                                parent: request.ParentKey);

            if (childs.Any() == true)
            {
                return (childs, GetConfigRowSelectConditionTypeEnum.ByApplication);
            }

            childs = await _configRowRepository.GetChildKeysWithoutApplicationAsync(environment: request.Environment, parent: request.ParentKey);

            if (childs.Any() == true)
            {
                return (childs, GetConfigRowSelectConditionTypeEnum.WithoutApplication);
            }

            return null;
        }

        private async Task<(List<string> Sections, GetConfigRowSelectConditionTypeEnum SelectedConditionType)?> GetSectionsAsync(GetPathChildsAndSectionsKeysRequest request)
        {
            var sections = await _configRowRepository.GetSectionsByApplicationAsync(environment: request.Environment,
                                            application: request.Application,
                                            parent: request.ParentKey);

            if (sections.Any() == true)
            {
                return (sections, GetConfigRowSelectConditionTypeEnum.ByApplication);
            }

            sections = await _configRowRepository.GetSectionsWithoutApplicationAsync(environment: request.Environment,
                                            parent: request.ParentKey);

            if (sections.Any() == true)
            {
                return (sections, GetConfigRowSelectConditionTypeEnum.WithoutApplication);
            }

            return null;
        }
    }
}
