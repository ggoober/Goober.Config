using Goober.Config.Api.Models;
using Microsoft.Extensions.Configuration;
using System;

namespace Goober.Config.Api
{
    class HttpConfigSource : IConfigurationSource
    {
        private readonly HttpConfigParameters _httpConfigParameters;
        private readonly IServiceProvider _serviceProvider;

        public HttpConfigSource(HttpConfigParameters httpConfigParameters, IServiceProvider serviceProvider)
        {
            _httpConfigParameters = httpConfigParameters;
            _serviceProvider = serviceProvider;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new HttpConfigProvider(httpConfigParameters: _httpConfigParameters, serviceProvider: _serviceProvider);
        }
    }
}
