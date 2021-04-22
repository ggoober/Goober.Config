using Goober.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Goober.Config.DAL.MsSql;

namespace Goober.Config.WebApi
{
    public class Startup : Goober.Web.BaseApiStartup
    {
        public Startup() 
            : base(configSettings: 
                    new BaseStartupConfigSettings {
                        AppSettingsFileName = "appsettings.json",
                        ConfigApiEnvironmentAndHostMappings = null,
                        IsAppSettingsFileOptional = false 
                    }, 
                    swaggerSettings: null,
                    memoryCacheSizeLimitInMB: null) 
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
            services.RegisterConfigMsSqlDbContext(Configuration);
            services.RegisterRepositories();
        }
    }
}
