using Goober.Config.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Goober.Config.DAL.Repositories
{
    public interface IConfigRowRepository
    {
        Task<List<ConfigRowResult>> GetByApplicationAsync(string environment, string application, string key, string parent);

        Task<List<ConfigRowResult>> GetWithoutApplicationAsync(string environment, string key, string parent);

        Task<List<ConfigRowResult>> GetIgnoringApplicationAsync(string environment, string key, string parent);
    }
}
