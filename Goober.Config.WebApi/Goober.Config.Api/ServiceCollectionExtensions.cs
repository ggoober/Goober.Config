using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Goober.Config.Api
{
    public static class ServiceCollectionExtensions
    {
        private const string EnvironmentKey = "ASPNETCORE_ENVIRONMENT";
        private const string DevelopmentEnvironment = "Development";

        public static IConfigurationBuilder AddApiConfiguration(this IConfigurationBuilder builder,
            Dictionary<string, string> environmentConfigApiSchemeAndHosts,
            string applicationName = null)
        {
            string environment = Environment.GetEnvironmentVariable(EnvironmentKey) ?? DevelopmentEnvironment;

            environmentConfigApiSchemeAndHosts.TryGetValue(environment, out var configApiSchemeAndHost);

            if (string.IsNullOrEmpty(configApiSchemeAndHost) == true)
            {
                throw new InvalidOperationException($"{nameof(configApiSchemeAndHost)} is empty");
            }

            var correctedApplicationName = applicationName ?? Assembly.GetEntryAssembly().GetName().Name;

            builder.Add(
                new HttpConfigSource(
                            new Models.HttpConfigParameters
                            {
                                Environment = environment,
                                ApiSchemeAndHost = configApiSchemeAndHost,
                                ApplicationName = correctedApplicationName
                            }));

            return builder;
        }
    }
}
