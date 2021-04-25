using Goober.Config.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Goober.Config.DAL.Repositories
{
    public interface IConfigRowRepository
    {
        
        Task<List<ConfigRowResult>> GetByApplicationAsync(string environment, string application, string key, string parent);

        Task<List<ConfigRowResult>> GetExcludeApplicationAsync(string environment, string key, string parent, bool anyApplication);

        Task<List<string>> GetChildKeysByApplicationAsync(string environment, string application, string parent);
        
        Task<List<string>> GetChildKeysWithoutApplicationAsync(string environment, string parent);
        
        Task<List<string>> GetSectionsByApplicationAsync(string environment, string application, string parent);
        
        Task<List<string>> GetSectionsWithoutApplicationAsync(string environment, string parent);
        
        Task<ConfigRowResult> InsertAsync(ConfigRowResult configRow);
        
        Task<ConfigRowResult> UpdateAsync(ConfigRowResult configRow);
        
        Task DeleteAsync(ConfigRowResult configRow);

    }
}