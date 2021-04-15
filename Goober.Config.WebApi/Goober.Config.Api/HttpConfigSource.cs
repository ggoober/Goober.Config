using Goober.Config.Api.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace Goober.Config.Api
{
    class HttpConfigSource : IConfigurationSource
    {
        private readonly HttpConfigParameters _httpConfigParameters;

        private readonly IHttpClientFactory _httpClientFactory;

        public HttpConfigSource(HttpConfigParameters httpConfigParameters)
        {
            _httpConfigParameters = httpConfigParameters;

            var sc = new ServiceCollection();
            sc.AddHttpClient();
            var sp = sc.BuildServiceProvider();
            _httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new HttpConfigProvider(httpConfigParameters: _httpConfigParameters, httpClientFactory: _httpClientFactory);
        }
    }
}
