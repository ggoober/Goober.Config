using Goober.Config.DAL.Entities;
using Goober.Config.DAL.MsSql.DbContext;
using Goober.Config.DAL.Repositories;
using Goober.EntityFramework.Common.Implementation;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goober.Config.DAL.MsSql.Repositories.Implementation
{
    class ConfigRowRepository : BaseRepository<ConfigRow>, IConfigRowRepository
    {
        public ConfigRowRepository(IConfigDbContext dbContext)
            : base(dbContext.ConfigRows)
        {

        }

        public async Task<List<ConfigRowResult>> GetByApplicationAsync(string environment, string application, string key, string parent)
        {
            var query = DbSet.Where(x => x.Environment == environment && x.Application == application && x.Key == key);

            query = SetQueryParentKeyCondition(query, parent);

            var res = await query.ToListAsync();

            return res.Select(x => ConvertToConfigRowResult(x)).ToList();
        }

        public async Task<List<ConfigRowResult>> GetIgnoringApplicationAsync(string environment, string key, string parent)
        {
            var query = DbSet.Where(x => x.Environment == environment && x.Key == key);
            query = SetQueryParentKeyCondition(query, parent);

            var res = await query.ToListAsync();

            return res.Select(x => ConvertToConfigRowResult(x)).ToList();
        }

        public async Task<List<ConfigRowResult>> GetWithoutApplicationAsync(string environment, string key, string parent)
        {
            var query = DbSet.Where(x => x.Environment == environment && x.Key == key && x.Application == null);
            query = SetQueryParentKeyCondition(query, parent);

            var res = await query.ToListAsync();

            return res.Select(x => ConvertToConfigRowResult(x)).ToList();
        }

        private static IQueryable<ConfigRow> SetQueryParentKeyCondition(IQueryable<ConfigRow> query, string parent)
        {
            if (string.IsNullOrEmpty(parent) == true)
            {
                query = query.Where(x => x.ParentKey == null);
            }
            else
            {
                query = query.Where(x => x.ParentKey == parent);
            }

            return query;
        }

        private static ConfigRowResult ConvertToConfigRowResult(ConfigRow configRow)
        {
            return new ConfigRowResult
            {
                Id = configRow.Id,
                Application = configRow.Application,
                ChangedUserName = configRow.ChangedUserName,
                Environment = configRow.Environment,
                Key = configRow.Key,
                ParentKey = configRow.ParentKey,
                RowChangedDate = configRow.RowChangedDate,
                RowCreatedDate = configRow.RowCreatedDate,
                Value = configRow.Value
            };
        }
    }
}
