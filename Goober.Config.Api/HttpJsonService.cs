using Goober.CommonModels;
using Goober.Config.Api.Models.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Goober.Config.Api
{
    [ServiceCollectionIgnoreRegistrationAttribute]
    internal class HttpJsonService
    {
        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter(
                    namingStrategy: new CamelCaseNamingStrategy(processDictionaryKeys: true, overrideSpecifiedNames: false, processExtensionDataNames: true),
                    allowIntegerValues: true)
            },
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Culture = System.Globalization.CultureInfo.InvariantCulture,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateParseHandling = DateParseHandling.DateTime
        };

        private readonly IHttpClientFactory _httpClientFactory;
        private const string ApplicationJsonContentTypeValue = "application/json";

        public HttpJsonService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private async Task<string> ExecutePostReturnStringInternalAsync<TRequest>(
            HttpRequestContextModel<TRequest> requestContext,
            int timeoutInMilliseconds,
            long maxResponseContentLength)
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.Timeout = TimeSpan.FromMilliseconds(timeoutInMilliseconds);

                var httpRequest = GenerateHttpRequestMessage(
                    requestUrl: requestContext.Url,
                    httpMethodType: HttpMethod.Post,
                    authenticationHeaderValue: requestContext.AuthenticationHeaderValue,
                    headerValues: requestContext.HeaderValues,
                    responseMediaTypes: new List<string> { ApplicationJsonContentTypeValue });

                var strJsonContent = Serialize(requestContext.RequestContent, requestContext.JsonSerializerSettings);

                httpRequest.Content = new StringContent(content: strJsonContent, Encoding.UTF8, mediaType: ApplicationJsonContentTypeValue);

                var httpResponse = await httpClient.SendAsync(httpRequest);

                if (httpResponse.StatusCode == HttpStatusCode.NoContent)
                {
                    return default;
                }

                var ret = await GetResponseStringAndProcessResponseStatusCodeAsync(
                    httpResponse: httpResponse,
                    loggingRequestContext: requestContext,
                    maxResponseContentLength: maxResponseContentLength);

                return ret;
            }
        }

        public async Task<TResponse> ExecutePostAsync<TResponse, TRequest>(
            string schemeAndHost,
            string urlPath,
            TRequest request,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = 60000,
            long maxResponseContentLength = 300 * 1024)
        {
            var url = BuildUrl(schemeAndHost: schemeAndHost, urlPath: urlPath);

            var requestContext = new HttpRequestContextModel<TRequest>
            {
                Url = url,
                HttpMethod = HttpMethod.Post,
                QueryParameters = null,
                AuthenticationHeaderValue = authenticationHeaderValue,
                HeaderValues = headerValues,
                JsonSerializerSettings = jsonSerializerSettings ?? _jsonSerializerSettings,
                RequestContent = request
            };

            var strRet = await ExecutePostReturnStringInternalAsync(
                requestContext: requestContext,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength: maxResponseContentLength);

            if (string.IsNullOrEmpty(strRet) == true)
                return default;

            return Deserialize<TResponse, TRequest>(
                value: strRet,
                jsonSerializerSettings: requestContext.JsonSerializerSettings,
                loggingRequestContext: requestContext);
        }
        
        #region private methods

        private static string Serialize(object value, JsonSerializerSettings serializerSettings = null)
        {
            return JsonConvert.SerializeObject(value, serializerSettings ?? _jsonSerializerSettings);
        }

        private static TTarget Deserialize<TTarget, TRequest>(string value,
            JsonSerializerSettings jsonSerializerSettings,
            HttpRequestContextModel<TRequest> loggingRequestContext)
        {
            try
            {
                return JsonConvert.DeserializeObject<TTarget>(value, jsonSerializerSettings ?? _jsonSerializerSettings);
            }
            catch (Exception exc)
            {
                throw new WebException(
                    message: $"Can't deserialize to type = \"{typeof(TTarget).Name}\", message = \"{exc.Message}\" from value = \"{value}\", request: {Serialize(loggingRequestContext, loggingRequestContext.JsonSerializerSettings)}",
                    innerException: exc);
            }
        }

        private static async Task<string> GetResponseStringAndProcessResponseStatusCodeAsync<TRequest>(HttpResponseMessage httpResponse,
            HttpRequestContextModel<TRequest> loggingRequestContext,
            long maxResponseContentLength)
        {
            var responseStringResult = await ReadContentWithMaxSizeRetrictionAsync(httpResponse.Content,
                encoding: Encoding.UTF8,
                maxSize: maxResponseContentLength);

            if (httpResponse.StatusCode == HttpStatusCode.OK
                || httpResponse.StatusCode == HttpStatusCode.Accepted
                || httpResponse.StatusCode == HttpStatusCode.Created)
            {
                if (responseStringResult.IsReadToTheEnd == true)
                {
                    return responseStringResult.StringResult.ToString();
                }

                throw new WebException($"Response content length is grater then {maxResponseContentLength}, request: {Serialize(loggingRequestContext, loggingRequestContext.JsonSerializerSettings)} ");
            }

            var exception = new WebException($"Request fault with statusCode = {httpResponse.StatusCode}, response: {responseStringResult.StringResult}, request: {Serialize(loggingRequestContext, loggingRequestContext.JsonSerializerSettings)}");

            if (responseStringResult.IsReadToTheEnd == false)
            {
                responseStringResult.StringResult.AppendLine();
                responseStringResult.StringResult.Append($"<<< NOT END, response size is greter than {maxResponseContentLength}");
            }

            throw exception;
        }

        private static async Task<(bool IsReadToTheEnd, StringBuilder StringResult)> ReadContentWithMaxSizeRetrictionAsync(HttpContent httpContent,
                    Encoding encoding,
                    long maxSize,
                    int bufferSize = 1024)
        {
            using (var stream = await httpContent.ReadAsStreamAsync())
            {
                if (stream.CanRead == false)
                {
                    throw new InvalidOperationException("stream is not ready to ready");
                }

                var totalBytesRead = 0;

                var sbResult = new StringBuilder();

                byte[] buffer = new byte[bufferSize];
                var bytesRead = await stream.ReadAsync(buffer, 0, bufferSize);

                while (bytesRead > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead > maxSize)
                    {
                        return (false, sbResult);
                    }

                    sbResult.Append(encoding.GetString(bytes: buffer, index: 0, count: bytesRead));

                    bytesRead = await stream.ReadAsync(buffer, 0, bufferSize);
                }

                return (true, sbResult);
            }
        }

        private static string BuildUrl(string schemeAndHost, string urlPath)
        {
            var baseUri = new UriBuilder(new Uri(new Uri(schemeAndHost), urlPath));

            return new UriBuilder(scheme: baseUri.Scheme,
                                                host: baseUri.Host,
                                                port: baseUri.Port,
                                                path: baseUri.Path,
                                                extraValue: baseUri.Query).Uri.ToString();
        }

        private static HttpRequestMessage GenerateHttpRequestMessage(
            string requestUrl,
            HttpMethod httpMethodType,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            List<string> responseMediaTypes = null)
        {
            var ret = new HttpRequestMessage();

            if (responseMediaTypes != null && responseMediaTypes.Any())
            {
                foreach (var iResponseMediaType in responseMediaTypes)
                {
                    ret.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(iResponseMediaType));
                }
            }

            if (authenticationHeaderValue != null)
            {
                ret.Headers.Authorization = authenticationHeaderValue;
            }

            if (headerValues != null && headerValues.Any())
            {
                foreach (var item in headerValues)
                {
                    ret.Headers.Add(item.Key, item.Value);
                }
            }

            ret.Method = httpMethodType;
            ret.RequestUri = new Uri(requestUrl);

            return ret;
        }

        #endregion

    }
}
