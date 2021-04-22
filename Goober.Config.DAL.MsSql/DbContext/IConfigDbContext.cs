using Goober.Config.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Goober.Config.DAL.MsSql.DbContext
{
    internal interface IConfigDbContext
    {
        DbSet<ConfigRow> ConfigRows { get; set; }
    }
}