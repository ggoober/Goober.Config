using Goober.Config.Api.Models;
using Goober.Config.Api.Models.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Goober.Config.Api
{
    internal class HttpConfigProvider : IConfigurationProvider
    {
        private class CacheResult<T>
        {
            public T TargetObject { get; set; }

            public DateTime? RefreshTime { get; set; }
        }

        private readonly ConfigurationReloadToken _configurationReloadToken;
        private readonly HttpConfigParameters _httpConfigParameters;
        private readonly IServiceProvider _serviceProvider;
        private readonly HttpJsonService _httpJsonService;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<HttpConfigProvider> _logger;

        private const string GetChildKeysAndSectionsPath = "api/get-childs-keys-and-sections";
        private const string GetConfigRaw = "api/get-config-raw";

        public HttpConfigProvider(HttpConfigParameters httpConfigParameters, IServiceProvider serviceProvider)
        {
            _configurationReloadToken = new ConfigurationReloadToken();

            _httpConfigParameters = httpConfigParameters;
            _serviceProvider = serviceProvider;

            var httpClientFactory = _serviceProvider.GetRequiredService<IHttpClientFactory>();

            _httpJsonService = new HttpJsonService(httpClientFactory: httpClientFactory);
            _memoryCache = _serviceProvider.GetRequiredService<IMemoryCache>();
            _logger = _serviceProvider.GetRequiredService<ILogger<HttpConfigProvider>>();
        }

        public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
        {
            var ealrierKeysString = earlierKeys != null ? string.Join(";", earlierKeys) : string.Empty;

            var cacheKey = $"HttpConfigProvider.GetChildKeys({ealrierKeysString}, {parentPath})";

            var ret = GetCached(cacheKey: cacheKey,
                refreshTimeInMinutes: _httpConfigParameters?.ConfigApiParameters?.CacheRefreshTimeInMinutes,
                expirationTimeInMinutes: _httpConfigParameters?.ConfigApiParameters?.CacheExpirationTimeInMinutes,
                func: () => GetChildKeysInternal(earlierKeys, parentPath));

            return ret;
        }

        private List<string> GetChildKeysInternal(IEnumerable<string> earlierKeys, string parentPath)
        {
            var keys = earlierKeys != null ? string.Join(",", earlierKeys.OrderBy(x => x)) : string.Empty;

            var task = Task.Run(async() => await _httpJsonService.ExecutePostAsync<GetPathChildsAndSectionsKeysResponse, GetPathChildsAndSectionsKeysRequest>(
                                    schemeAndHost: _httpConfigParameters.ApiSchemeAndHost,
                                    urlPath: GetChildKeysAndSectionsPath,
                                    request: new GetPathChildsAndSectionsKeysRequest
                                    {
                                        Application = _httpConfigParameters.ApplicationName,
                                        Environment = _httpConfigParameters.Environment,
                                        ParentKey = parentPath
                                    }));

            var configResult = task.Result;

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
            
            value = GetCached(cacheKey: cacheKey,
                refreshTimeInMinutes: _httpConfigParameters?.ConfigApiParameters?.CacheRefreshTimeInMinutes,
                expirationTimeInMinutes: _httpConfigParameters?.ConfigApiParameters?.CacheExpirationTimeInMinutes,
                func: () => TryGetInternal(key));

            if (string.IsNullOrEmpty(value) == true)
            {
                return false;
            }

            return true;
        }

        private string TryGetInternal(string key)
        {
            var splittedKeys = key.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

            var calcKey = key;

            string parentKey = null;

            if (splittedKeys.Count > 1)
            {
                calcKey = splittedKeys.Last();

                parentKey = string.Join(":", splittedKeys.Take(splittedKeys.Count - 1));
            }

            var task = Task.Run(async () => await _httpJsonService.ExecutePostAsync<GetConfigRawResponseModel, GetConfigRawRequestModel>(
                                schemeAndHost: _httpConfigParameters.ApiSchemeAndHost,
                                urlPath: GetConfigRaw,
                                request: new GetConfigRawRequestModel
                                {
                                    Application = _httpConfigParameters.ApplicationName,
                                    Environment = _httpConfigParameters.Environment,
                                    Key = calcKey,
                                    ParentKey = parentKey
                                }));

            var result = task.Result;

            return result?.Value;
        }

        private T GetCached<T>(string cacheKey, int? refreshTimeInMinutes, int? expirationTimeInMinutes, Func<T> func)
        {
            var cachedResult = _memoryCache.Get(cacheKey) as CacheResult<T>;

            var currentDateTime = DateTime.Now;

            if (cachedResult != null)
            {
                if (refreshTimeInMinutes.HasValue == true
                    && cachedResult.RefreshTime <= currentDateTime)
                {
                    try
                    {
                        cachedResult.TargetObject = func();
                        cachedResult.RefreshTime = currentDateTime.AddMinutes(refreshTimeInMinutes.Value);
                    }
                    catch (Exception exc)
                    {
                        _logger.LogError(exception: exc, message: $"Error while refreshing cache");
                    }
                }

                if (cachedResult.TargetObject == null)
                    return default(T);

                return cachedResult.TargetObject;
            }

            var expensiveObject = func();
            var absoluteRefreshDateTime = refreshTimeInMinutes.HasValue == true ? currentDateTime.AddMinutes(refreshTimeInMinutes.Value) : (DateTime?)null;
            var newCachedResult = new CacheResult<T>
            {
                RefreshTime = absoluteRefreshDateTime,
                TargetObject = expensiveObject
            };

            if (expirationTimeInMinutes.HasValue == true)
            {
                var absoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(expirationTimeInMinutes.Value));

                _memoryCache.Set(key: cacheKey, value: newCachedResult, absoluteExpiration: absoluteExpiration);
            }
            else
            {
                _memoryCache.Set(key: cacheKey, value: newCachedResult);
            }

            if (newCachedResult.TargetObject == null)
                return default(T);

            return newCachedResult.TargetObject;
        }
    }
}
