using Goober.Core.Extensions;
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

        #region IConfigRowRepository

        public async Task<List<ConfigRowResult>> GetByApplicationAsync(string environment, string application, string key, string parent)
        {
            var query = DbSet.Where(x => x.Environment == environment && x.Application == application && x.Key == key);

            query = SetQueryParentKeyCondition(query, parent);

            var res = await query.ToListAsync();

            return res.Select(x => ConvertToConfigRowResult(x)).ToList();
        }

        public async Task<List<ConfigRowResult>> GetExcludeApplicationAsync(string environment, string key, string parent, bool anyApplication)
        {
            var query = DbSet.Where(x => x.Environment == environment && x.Key == key);

            if (anyApplication == false)
            {
                query = query.Where(x => x.Application == null);
            }

            query = SetQueryParentKeyCondition(query, parent);

            var res = await query.ToListAsync();

            return res.Select(x => ConvertToConfigRowResult(x)).ToList();
        }

        public async Task<List<string>> GetChildKeysByApplicationAsync(string environment, string application, string parent)
        {
            var query = DbSet.Where(x => x.Environment == environment && x.Application == application && x.ParentKey == parent);
            var res = await query.Select(x => x.Key).Distinct().ToListAsync();

            return res;
        }

        public async Task<List<string>> GetChildKeysWithoutApplicationAsync(string environment, string parent)
        {
            var query = DbSet.Where(x => x.Environment == environment && x.Application == null && x.ParentKey == parent);

            var res = await query.Select(x => x.Key).Distinct().ToListAsync();

            return res;
        }

        public async Task<List<string>> GetSectionsByApplicationAsync(string environment, string application, string parent)
        {
            var query = DbSet.Where(x => x.Environment == environment && x.Application == application && x.ParentKey.StartsWith(parent) && x.ParentKey != parent);
            var res = await query.Select(x => x.ParentKey).Distinct().ToListAsync();

            return res;
        }

        public async Task<List<string>> GetSectionsWithoutApplicationAsync(string environment, string parent)
        {
            var query = DbSet.Where(x => x.Environment == environment && x.Application == null && x.ParentKey.StartsWith(parent) && x.ParentKey != parent);

            var res = await query.Select(x => x.ParentKey).Distinct().ToListAsync();

            return res;
        }

        #endregion

        #region IBaseRepository


        public async Task<ConfigRowResult> InsertAsync(ConfigRowResult configRow)
        {
            var newRec = GenerateConfigRowEntity(configRow);

            await this.InsertAsync(newRec);

            configRow.Id = newRec.Id;
            configRow.RowCreatedDate = newRec.RowCreatedDate;
            configRow.RowChangedDate = newRec.RowChangedDate;

            return configRow;
        }

        public async Task<ConfigRowResult> UpdateAsync(ConfigRowResult configRow)
        {
            var updateRec = GenerateConfigRowEntity(configRow);

            await this.UpdateAsync(updateRec);

            configRow.RowChangedDate = updateRec.RowChangedDate;
            configRow.RowCreatedDate = updateRec.RowCreatedDate;

            return configRow;
        }

        public async Task DeleteAsync(ConfigRowResult configRow)
        {
            var deleteRec = await DbSet.FirstOrDefaultAsync(x => x.Id == configRow.Id);
            deleteRec.RequiredNotNull(nameof(deleteRec), context: configRow);

            await this.DeleteAsync(deleteRec);
        }

        #endregion

        #region private methods

        private static ConfigRow GenerateConfigRowEntity(ConfigRowResult configRow)
        {
            return new ConfigRow
            {
                Application = configRow.Application,
                ChangedUserName = configRow.ChangedUserName,
                Environment = configRow.Environment,
                Id = configRow.Id,
                Key = configRow.Key,
                ParentKey = configRow.ParentKey,
                RowChangedDate = configRow.RowChangedDate,
                RowCreatedDate = configRow.RowCreatedDate,
                Value = configRow.Value
            };
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

        #endregion
    }
}
