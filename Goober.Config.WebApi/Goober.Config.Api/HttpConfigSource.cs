using Goober.Config.Api.Models;
using Microsoft.Extensions.Configuration;

namespace Goober.Config.Api
{
    class HttpConfigSource : IConfigurationSource
    {
        private readonly HttpConfigParameters _httpConfigParameters;

        public HttpConfigSource(HttpConfigParameters httpConfigParameters)
        {
            _httpConfigParameters = httpConfigParameters;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new HttpConfigProvider(httpConfigParameters: _httpConfigParameters);
        }
    }
}
