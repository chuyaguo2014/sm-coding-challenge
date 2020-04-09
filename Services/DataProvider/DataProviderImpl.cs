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

        private List<PlayerModel> getPlayersFromResponse(String[] idArray, DataResponseModel dataResponse)
        {
            var returnList = new List<PlayerModel>();
            foreach (var player in dataResponse.Rushing)
            {
                Console.WriteLine("this is our player right now: {0}", player);
                if (idArray.Contains(player.Id))
                {
                    returnList.Add(player);
                }
            }
            foreach (var player in dataResponse.Passing)
            {
                if (idArray.Contains(player.Id))
                {
                    returnList.Add(player);
                }
            }
            foreach (var player in dataResponse.Receiving)
            {
                if (idArray.Contains(player.Id))
                {
                    returnList.Add(player);
                }
            }
            foreach (var player in dataResponse.Receiving)
            {
                if (idArray.Contains(player.Id))
                {
                    returnList.Add(player);
                }
            }
            foreach (var player in dataResponse.Kicking)
            {
                if (idArray.Contains(player.Id))
                {
                    returnList.Add(player);
                }
            }
            return returnList;
        }
    }
}
