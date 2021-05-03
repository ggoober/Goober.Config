using Goober.Config.DAL.PostgreSql.DbContext;
using Goober.Config.DAL.PostgreSql.DbContext.Implementation;
using Goober.Config.DAL.PostgreSql.Repositories.Implementation;
using Goober.Config.DAL.Repositories;
using Goober.EntityFramework.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Goober.Config.DAL.PostgreSql
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterConfigPostgreSqlDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterPostgreSqlDbContext<IConfigDbContext, ConfigDbContext>(() => configuration.GetConnectionString("ConfigDB"));
        }

        public static void RegisterConfigPostgreSqlRepositories(this IServiceCollection services)
        {
            services.AddScoped<IConfigRowRepository, ConfigRowRepository>();
        }
    }
}
