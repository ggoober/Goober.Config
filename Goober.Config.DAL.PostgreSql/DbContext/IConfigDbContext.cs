using Goober.Config.DAL.PostgreSql.Entities;
using Microsoft.EntityFrameworkCore;

namespace Goober.Config.DAL.PostgreSql.DbContext
{
    internal interface IConfigDbContext
    {
        DbSet<ConfigRow> ConfigRows { get; set; }
    }
}