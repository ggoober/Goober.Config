﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Goober.Config.Api.Models.Internal
{
    internal class HttpRequestContextModel<TRequest>
    {
        public string Url { get; set; }

        public HttpMethod HttpMethod { get; set; }

        public List<KeyValuePair<string, string>> QueryParameters { get; set; }

        public AuthenticationHeaderValue AuthenticationHeaderValue { get; set; }

        public List<KeyValuePair<string, string>> HeaderValues { get; set; }

        public TRequest RequestContent { get; set; }

        public List<KeyValuePair<string, string>> FormContent { get; set; }

        public List<string> Files { get; set; }

        [JsonIgnore]
        public JsonSerializerSettings JsonSerializerSettings { get; set; }
    }
}
