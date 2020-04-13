using System;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using sm_coding_challenge.Models;
using System.Collections.Generic;
using System.Linq;
using System.IO;

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
            if (!string.IsNullOrEmpty(ids))
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

        public Dictionary<String, List<PlayerModel>> GetLatestPlayers(string ids)
        {
            var idList = ids.Split(',');
            string path = Directory.GetCurrentDirectory();
            var historicalDataDirectory = new DirectoryInfo(path + "/Services/DataProvider/historicalData");

            var latestFile = (from f in historicalDataDirectory.GetFiles()
                              orderby f.LastWriteTime descending
                              select f).First();

            using (StreamReader reader = new StreamReader(latestFile.ToString()))
            {
                string content = reader.ReadToEnd();
                var dataResponse = JsonConvert.DeserializeObject<DataResponseModel>(content, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });

                var rushingDictionary = generateDictionary(dataResponse.Rushing);
                var rushingPlayers = lookUpPlayersInDictionary(idList, rushingDictionary);

                var passingDictionary = generateDictionary(dataResponse.Passing);
                var passingPlayers = lookUpPlayersInDictionary(idList, passingDictionary);

                var receivingDictionary = generateDictionary(dataResponse.Receiving);
                var receivingPlayers = lookUpPlayersInDictionary(idList, receivingDictionary);

                var kickingingDictionary = generateDictionary(dataResponse.Kicking);
                var kickingPlayers = lookUpPlayersInDictionary(idList, kickingingDictionary);

                var returnDictionary = new Dictionary<String, List<PlayerModel>>();
                if (rushingPlayers.Count > 0)
                {
                    returnDictionary.Add("rushing", rushingPlayers);
                }

                if (passingPlayers.Count > 0)
                {
                    returnDictionary.Add("passing", passingPlayers);
                }

                if (receivingPlayers.Count > 0)
                {
                    returnDictionary.Add("receiving", receivingPlayers);
                }

                if (kickingPlayers.Count > 0)
                {
                    returnDictionary.Add("kicking", kickingPlayers);
                }
                return returnDictionary;
            }
        }

        /// <summary>
        /// Search dataResponse (containing all the players) for those with the given ids
        /// </summary>
        /// <param name="idArray"></param>
        /// <param name="dataResponse"></param>
        /// <returns></returns>
        private List<PlayerModel> getPlayersFromResponse(String[] idArray, DataResponseModel dataResponse)
        {
            Dictionary<String, PlayerModel> playerDictionary = getAllPlayersDictionary(dataResponse);
            return lookUpPlayersInDictionary(idArray, playerDictionary);
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

            return generateDictionary(allPlayers);
        }

        private Dictionary<String, PlayerModel> generateDictionary(List<PlayerModel> playerList)
        {
            return playerList.GroupBy(p => p.Id, StringComparer.OrdinalIgnoreCase).ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
        }

        private List<PlayerModel> lookUpPlayersInDictionary(String[] idArray, Dictionary<String, PlayerModel> playerDictionary)
        {
            var distinctIdArray = idArray.Distinct().ToArray();
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
    }
}
