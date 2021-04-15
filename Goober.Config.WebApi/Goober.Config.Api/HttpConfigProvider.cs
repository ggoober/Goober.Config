using Goober.Config.Api.Models;
using Goober.Config.Api.Models.Internal;
using Goober.Config.Api.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Goober.Config.Api
{
    internal class HttpConfigProvider : IConfigurationProvider
    {
        private readonly ConfigurationReloadToken _configurationReloadToken;
        private readonly HttpConfigParameters _httpConfigParameters;

        private const string GetChildKeysAndSectionsPath = "api/get-childs-keys-and-sections";
        private const string GetConfigRaw = "api/get-config-raw";

        public HttpConfigProvider(HttpConfigParameters httpConfigParameters)
        {
            _configurationReloadToken = new ConfigurationReloadToken();

            _httpConfigParameters = httpConfigParameters;
        }

        public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
        {
            var keys = earlierKeys != null ? string.Join(",", earlierKeys.OrderBy(x => x)) : string.Empty;

            var configResult = HttpUtils.ExecutePostAsync<GetPathChildsAndSectionsKeysResponse, GetPathChildsAndSectionsKeysRequest>(
                schemeAndHost: _httpConfigParameters.ApiSchemeAndHost,
                urlPath: GetChildKeysAndSectionsPath,
                request: new GetPathChildsAndSectionsKeysRequest
                {
                    Application = _httpConfigParameters.ApplicationName,
                    Environment = _httpConfigParameters.Environment,
                    ParentKey = parentPath
                }).RunSync();

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
            value = TryGetInternal(key);
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

            var ret = HttpUtils.ExecutePostAsync<GetConfigRawResponseModel, GetConfigRawRequestModel>(
                schemeAndHost: _httpConfigParameters.ApiSchemeAndHost,
                urlPath: GetConfigRaw,
                request: new GetConfigRawRequestModel
                {
                    Application = _httpConfigParameters.ApplicationName,
                    Environment = _httpConfigParameters.Environment,
                    Key = calcKey,
                    ParentKey = parentKey
                }).RunSync();

            return ret?.Value;
        }
    }
}
