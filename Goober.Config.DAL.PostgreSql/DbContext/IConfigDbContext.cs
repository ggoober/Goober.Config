using Goober.Config.DAL.PostgreSql.Entities;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Goober.Config.WebApi.Tests")]
namespace Goober.Config.DAL.PostgreSql.DbContext
{
    internal interface IConfigDbContext
    {
        DbSet<ConfigRow> ConfigRows { get; set; }
    }
}