using System.Threading.Tasks;
using sm_coding_challenge.Models;
using System.Collections.Generic;

namespace sm_coding_challenge.Services.DataProvider
{
    public interface IDataProvider
    {
        Task<List<PlayerModel>> GetPlayersByIds(string ids);
        public Dictionary<string, List<PlayerModel>> GetLatestPlayers(string ids);
    }
}
