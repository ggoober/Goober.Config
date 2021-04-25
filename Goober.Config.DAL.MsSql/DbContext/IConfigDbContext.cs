using Goober.Config.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Goober.Config.WebApi.Tests")]
namespace Goober.Config.DAL.MsSql.DbContext
{
    internal interface IConfigDbContext
    {
        DbSet<ConfigRow> ConfigRows { get; set; }
    }
}