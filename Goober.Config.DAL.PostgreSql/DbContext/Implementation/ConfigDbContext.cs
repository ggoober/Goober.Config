using Goober.EntityFramework.Common.Implementation;
using Microsoft.EntityFrameworkCore;
using Goober.Config.DAL.PostgreSql.Entities;

namespace Goober.Config.DAL.PostgreSql.DbContext.Implementation
{
    class ConfigDbContext : BaseDbContext, IConfigDbContext
    {
        public ConfigDbContext(DbContextOptions<ConfigDbContext> options) : base(options)
        {
        }

        public DbSet<ConfigRow> ConfigRows { get; set; }
    }
}
