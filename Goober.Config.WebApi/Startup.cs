using Goober.WebApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Goober.Config.WebApi
{
    public class Startup : Goober.WebApi.BaseStartup
    {
        public Startup() 
            : base(configSettings: 
                    new BaseStartupConfigSettings {
                        AppSettingsFileName = "appsettings.json",
                        ConfigApiEnvironmentAndHostMappings = null,
                        IsAppSettingsFileOptional = false 
                    }, 
                    swaggerSettings: null) 
        {
            //do nothing
        }

        protected override void ConfigurePipelineAfterExceptionsHandling(IApplicationBuilder app)
        {
        }

        protected override void ConfigurePipelineAfterMvc(IApplicationBuilder app)
        {
        }

        protected override void ConfigureServiceCollections(IServiceCollection services)
        {
        }
    }
}
