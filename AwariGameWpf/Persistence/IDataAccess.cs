using System.Collections.Generic;
using System.Threading.Tasks;

namespace AwariGameWpf.Persistence
{
    public interface IDataAccess
    {
        Task SaveAsync(string path, Dictionary<int, int> bowls, bool player, bool again);

        Task<(Dictionary<int, int>, bool, bool)> LoadAsync(string path);
    }
}
