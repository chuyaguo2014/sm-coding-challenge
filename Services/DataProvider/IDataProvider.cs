using System.Threading.Tasks;
using sm_coding_challenge.Models;

namespace sm_coding_challenge.Services.DataProvider
{
    public interface IDataProvider
    {
        PlayerModel GetPlayerById_old(string id);
        Task<PlayerModel> GetPlayerById(string id);
    }
}
