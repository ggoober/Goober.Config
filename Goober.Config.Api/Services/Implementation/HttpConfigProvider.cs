using Goober.Caching.Services;
using Goober.Config.Api.Models;
using Goober.Config.Api.Models.Internal;
using Goober.Core.Extensions;
using Goober.Core.Services;
using Goober.Http.Services;
using Goober.Http.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goober.Config.Api.Services.Implementation
{
    internal class HttpConfigProvider : IHttpConfigProvider
    {
        private readonly ConfigurationReloadToken _configurationReloadToken;
        private readonly HttpConfigParameters _httpConfigParameters;
        private readonly IHttpJsonHelperService _httpJsonHelperService;
        private readonly ICacheProvider _cacheProvider;

        private const string GetChildKeysAndSectionsPath = "api/get-childs-keys-and-sections";
        private const string GetConfigRaw = "api/get-config-row";

        public HttpConfigProvider(HttpConfigParameters httpConfigParameters,
            IHttpJsonHelperService httpJsonHelperService,
            ICacheProvider cacheProvider)
        {
            _configurationReloadToken = new ConfigurationReloadToken();

            _httpConfigParameters = httpConfigParameters;
            _httpJsonHelperService = httpJsonHelperService;
            _cacheProvider = cacheProvider;
        }

        public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
        {
            var ealrierKeysString = earlierKeys != null ? string.Join(";", earlierKeys) : string.Empty;

            var cacheKey = $"HttpConfigProvider.GetChildKeys({ealrierKeysString}, {parentPath})";

            var ret = _cacheProvider.GetAsync(
                            cacheKey: cacheKey,
                            refreshTimeInMinutes: _httpConfigParameters?.CacheRefreshTimeInMinutes,
                            expirationTimeInMinutes: _httpConfigParameters?.CacheExpirationTimeInMinutes,
                            func: async () => await GetChildKeysInternalAsync(earlierKeys, parentPath))
                        .RunSync();

            return ret;
        }

        private async Task<List<string>> GetChildKeysInternalAsync(IEnumerable<string> earlierKeys, string parentPath)
        {
            var keys = earlierKeys != null ? string.Join(",", earlierKeys.OrderBy(x => x)) : string.Empty;

            var url = HttpUtils.BuildUrl(schemeAndHost: _httpConfigParameters.ApiSchemeAndHost,
                urlPath: GetChildKeysAndSectionsPath);

            var configResult = await _httpJsonHelperService.ExecutePostAsync<GetPathChildsAndSectionsKeysResponse, GetPathChildsAndSectionsKeysRequest>(
                                    url: url,
                                    request: new GetPathChildsAndSectionsKeysRequest
                                    {
                                        Application = _httpConfigParameters.ApplicationName,
                                        Environment = _httpConfigParameters.Environment,
                                        ParentKey = parentPath
                                    });

            var ret = new List<string>();

            if (configResult == null)
                return ret;

            ret.AddRange(configResult.Keys);

            foreach (var iSectionKey in configResult.Sections)
            {
                if (ret.Contains(iSectionKey) == true)
                    continue;

                ret.Add(iSectionKey);
            }

            return ret;
        }

        public IChangeToken GetReloadToken()
        {
            return _configurationReloadToken;
        }

        public void Load()
        {
        }

        public void Set(string key, string value)
        {
            throw new NotSupportedException();
        }

        public bool TryGet(string key, out string value)
        {
            var cacheKey = $"HttpConfigProvider.TryGet({key})";

            value = _cacheProvider.GetAsync(
                        cacheKey: cacheKey, 
                        refreshTimeInMinutes: _httpConfigParameters?.CacheRefreshTimeInMinutes,
                        expirationTimeInMinutes: _httpConfigParameters?.CacheExpirationTimeInMinutes,
                        func: async () => await TryGetInternalAsync(key))
                .RunSync();

            if (string.IsNullOrEmpty(value) == true)
            {
                return false;
            }

            return true;
        }

        private async Task<string> TryGetInternalAsync(string key)
        {
            var splittedKeys = key.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

            var calcKey = key;

            string parentKey = null;

            if (splittedKeys.Count > 1)
            {
                calcKey = splittedKeys.Last();

                parentKey = string.Join(":", splittedKeys.Take(splittedKeys.Count - 1));
            }

            var url = HttpUtils.BuildUrl(schemeAndHost: _httpConfigParameters.ApiSchemeAndHost,
                urlPath: GetConfigRaw);

            var result = await _httpJsonHelperService.ExecutePostAsync<GetConfigRowResponseModel, GetConfigRowRequestModel>(
                                url: url,
                                request: new GetConfigRowRequestModel
                                {
                                    Application = _httpConfigParameters.ApplicationName,
                                    Environment = _httpConfigParameters.Environment,
                                    Key = calcKey,
                                    ParentKey = parentKey
                                });

            return result?.Value;
        }
    }
}
