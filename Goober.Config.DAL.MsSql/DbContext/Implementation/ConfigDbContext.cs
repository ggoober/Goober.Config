using Goober.Config.DAL.Entities;
using Goober.EntityFramework.Common.Implementation;
using Microsoft.EntityFrameworkCore;

namespace Goober.Config.DAL.MsSql.DbContext.Implementation
{
    class ConfigDbContext : BaseDbContext, IConfigDbContext
    {
        public ConfigDbContext(DbContextOptions<ConfigDbContext> options) : base(options)
        {
        }

        public DbSet<ConfigRow> ConfigRows { get; set; }
    }
}
