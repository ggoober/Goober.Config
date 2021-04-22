using Goober.Config.Api.Models;
using Goober.Config.Api.Services;
using Goober.Config.Api.Services.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Goober.Config.Api
{
    public static class ServiceCollectionExtensions
    {
        private const string EnvironmentKey = "ASPNETCORE_ENVIRONMENT";
        private const string DevelopmentEnvironment = "Development";

        public static IConfigurationBuilder AddConfigApi(this IConfigurationBuilder configurationBuilder, Dictionary<string, string> environmentConfigApiSchemeAndHosts,
            IServiceCollection serviceCollection,
            int? cacheExpirationTimeInMinutes = null,
            int? cacheRefreshTimeInMinutes = null,
            string applicationName = null)
        {
            string environment = Environment.GetEnvironmentVariable(EnvironmentKey) ?? DevelopmentEnvironment;

            environmentConfigApiSchemeAndHosts.TryGetValue(environment, out var configApiSchemeAndHost);

            if (string.IsNullOrEmpty(configApiSchemeAndHost) == true)
            {
                throw new InvalidOperationException($"{nameof(configApiSchemeAndHost)} is empty");
            }

            var correctedApplicationName = applicationName ?? Assembly.GetEntryAssembly().GetName().Name;

            var httpConfigParameters = new HttpConfigParameters
            {
                Environment = environment,
                ApiSchemeAndHost = configApiSchemeAndHost,
                ApplicationName = correctedApplicationName,
                CacheExpirationTimeInMinutes = cacheExpirationTimeInMinutes,
                CacheRefreshTimeInMinutes = cacheRefreshTimeInMinutes
            };

            serviceCollection.AddSingleton(httpConfigParameters);
            serviceCollection.AddSingleton<IHttpConfigProvider, HttpConfigProvider>();

            var localServiceProvider = serviceCollection.BuildServiceProvider();
            configurationBuilder.Add(new HttpConfigSource(serviceProvider: localServiceProvider));

            return configurationBuilder;
        }
    }
}
