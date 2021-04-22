using Goober.Config.Api.Models;
using Goober.Config.Api.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Goober.Config.Api
{
    class HttpConfigSource : IConfigurationSource
    {
        private readonly IServiceProvider _serviceProvider;

        public HttpConfigSource(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return _serviceProvider.GetRequiredService<IHttpConfigProvider>();
        }
    }
}
