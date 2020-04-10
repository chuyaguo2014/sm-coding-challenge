using System;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using sm_coding_challenge.Models;
using System.Collections.Generic;
using System.Linq;


namespace sm_coding_challenge.Services.DataProvider
{
    public class DataProviderImpl : IDataProvider
    {
        public static TimeSpan Timeout = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Returns a distinct list of players from the api based on the given ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<List<PlayerModel>> GetPlayersByIds(string ids)
        {
            if (!String.IsNullOrEmpty(ids))
            {
                var handler = new HttpClientHandler()
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };

                var idList = ids.Split(',');
                using (var client = new HttpClient(handler))
                {
                    client.Timeout = Timeout;
                    var response = await client.GetAsync("https://gist.githubusercontent.com/RichardD012/a81e0d1730555bc0d8856d1be980c803/raw/3fe73fafadf7e5b699f056e55396282ff45a124b/basic.json");
                    var content = await response.Content.ReadAsStringAsync();
                    var dataResponse = JsonConvert.DeserializeObject<DataResponseModel>(content, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                    return getPlayersFromResponse(idList, dataResponse);
                }
            }
            return new List<PlayerModel>();

        }

        /// <summary>
        /// Search dataResponse (containing all the players) for those with the given ids
        /// </summary>
        /// <param name="idArray"></param>
        /// <param name="dataResponse"></param>
        /// <returns></returns>
        private List<PlayerModel> getPlayersFromResponse(String[] idArray, DataResponseModel dataResponse)
        {
            var distinctIdArray = idArray.Distinct().ToArray();
            Dictionary<String, PlayerModel> playerDictionary = getAllPlayersDictionary(dataResponse);

            var returnList = new List<PlayerModel>();

            foreach (string playerId in distinctIdArray)
            {
                PlayerModel foundPlayer;
                if (playerDictionary.TryGetValue(playerId, out foundPlayer))
                {
                    returnList.Add(foundPlayer);
                }
            }
            return returnList;
        }

        /// <summary>
        /// Based on the given dataResponse, generate a dictionary whose keys are player ids and whose values are the actual player instances
        /// thus all duplicate players are removed
        /// </summary>
        /// <param name="dataResponse"></param>
        /// <returns></returns>
        private Dictionary<String, PlayerModel> getAllPlayersDictionary(DataResponseModel dataResponse)
        {
            var allPlayers = new List<PlayerModel>(dataResponse.Rushing.Count + dataResponse.Passing.Count + dataResponse.Receiving.Count + dataResponse.Kicking.Count);
            allPlayers.AddRange(dataResponse.Rushing);
            allPlayers.AddRange(dataResponse.Passing);
            allPlayers.AddRange(dataResponse.Receiving);
            allPlayers.AddRange(dataResponse.Kicking);

            Dictionary<String, PlayerModel> playerDictionary = allPlayers.GroupBy(p => p.Id, StringComparer.OrdinalIgnoreCase).ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
            return playerDictionary;
        }
    }
}
