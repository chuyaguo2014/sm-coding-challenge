using System;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using sm_coding_challenge.Models;

namespace sm_coding_challenge.Services.DataProvider
{
    public class DataProviderImpl : IDataProvider
    {
        public static TimeSpan Timeout = TimeSpan.FromSeconds(30);

        public PlayerModel GetPlayerById_old(string id)
        {
            var handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            using (var client = new HttpClient(handler))
            {
                client.Timeout = Timeout;
                var response = client.GetAsync("https://gist.githubusercontent.com/RichardD012/a81e0d1730555bc0d8856d1be980c803/raw/3fe73fafadf7e5b699f056e55396282ff45a124b/basic.json").Result;
                var stringData = response.Content.ReadAsStringAsync().Result;
                var dataResponse = JsonConvert.DeserializeObject<DataResponseModel>(stringData, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                foreach(var player in dataResponse.Rushing)
                {
                    if(player.Id.Equals(id))
                    {
                        Console.Write("player id {0}\n", id);
                        return player;
                    }
                }
                foreach(var player in dataResponse.Passing)
                {
                    if(player.Id.Equals(id))
                    {
                        return player;
                    }
                }
                foreach(var player in dataResponse.Receiving)
                {
                    if(player.Id.Equals(id))
                    {
                        return player;
                    }
                }
                foreach(var player in dataResponse.Receiving)
                {
                    if(player.Id.Equals(id))
                    {
                        return player;
                    }
                }
                foreach(var player in dataResponse.Kicking)
                {
                    if(player.Id.Equals(id))
                    {
                        return player;
                    }
                }
            }
            return null;
        }
        public async Task<PlayerModel>  GetPlayerById(string id){
            var handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            using (var client = new HttpClient(handler))
            {
                Console.WriteLine("\n\n\n We are now in the async method...");
                client.Timeout = Timeout;
                var response = await client.GetAsync("https://gist.githubusercontent.com/RichardD012/a81e0d1730555bc0d8856d1be980c803/raw/3fe73fafadf7e5b699f056e55396282ff45a124b/basic.json");
                var content = await response.Content.ReadAsStringAsync();

                var dataResponse = JsonConvert.DeserializeObject<DataResponseModel>(content, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                return getPlaygerFromResponse(id, dataResponse);
            }
            Console.WriteLine("oh bummer we found no players!");
            return null;
        }
    
        private PlayerModel getPlaygerFromResponse(string id, DataResponseModel dataResponse){
            foreach(var player in dataResponse.Rushing)
                {
                    if(player.Id.Equals(id))
                    {
                        Console.Write("player id {0}\n", id);
                        return player;
                    }
                }
            foreach(var player in dataResponse.Passing)
            {
                if(player.Id.Equals(id))
                {
                    return player;
                }
            }
            foreach(var player in dataResponse.Receiving)
            {
                if(player.Id.Equals(id))
                {
                    return player;
                }
            }
            foreach(var player in dataResponse.Receiving)
            {
                if(player.Id.Equals(id))
                {
                    return player;
                }
            }
            foreach(var player in dataResponse.Kicking)
                {
                    if(player.Id.Equals(id))
                    {
                        return player;
                    }
                }
            return null;
        }
    }
}
