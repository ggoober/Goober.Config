using Goober.Config.DAL.MsSql.DbContext;
using Goober.Config.DAL.MsSql.DbContext.Implementation;
using Goober.Config.DAL.MsSql.Repositories.Implementation;
using Goober.Config.DAL.Repositories;
using Goober.EntityFramework.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Goober.Config.DAL.MsSql
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterConfigMsSqlDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterMsSqlDbContext<IConfigDbContext, ConfigDbContext>(() => configuration.GetConnectionString("ConfigDB"));
        }

        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IConfigRowRepository, ConfigRowRepository>();
        }
    }
}
