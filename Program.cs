using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

class Program
{
    static async Task Main()
    {
        // Substitua "SUA_CHAVE_DE_API" pela chave de API fornecida pela Riot Games e summonerName "Nome%20Invocador" pelo que você quer ver
        string apiKey = "SUA_CHAVE_DE_API";
        string summonerName = "Manopla%20Quebrada";

        // Substitua "REGIAO" pela região do servidor (por exemplo, "br1" para o servidor brasileiro)
        string region = "br1";
        string regionHistory = "AMERICAS";

        // Obtendo o ID do invocador usando o endpoint Summoner API
        SummonerData summonerData = await GetSummonerData(apiKey, summonerName, region);

        if (summonerData != null)
        {
            // Exibir informações formatadas
            Console.WriteLine($"Summoner Id: {summonerData.Id}");
            Console.WriteLine($"Summoner PuuId: {summonerData.Puuid}");
            Console.WriteLine($"Summoner Name: {summonerData.Name}");
            Console.WriteLine($"Profile Icon Id: {summonerData.ProfileIconId}");
            Console.WriteLine($"Summoner Level: {summonerData.SummonerLevel}\n");

            List<string> SummonerMatchIds = await GetMatchId(apiKey, summonerData, regionHistory);
            Console.WriteLine($"Summoner match Id: {SummonerMatchIds.Count}\n");

            List<RankedData> rankedDataList = await GetRankedData(apiKey, summonerData, region);

            if (rankedDataList != null && rankedDataList.Count > 0)
            {
                // Iterar sobre a lista de RankedData
                foreach (var rankedData in rankedDataList)
                {
                    Console.WriteLine($"Summoner ranked Id: {rankedData.LeagueId}");
                    Console.WriteLine($"Summoner ranked Type: {rankedData.QueueType}");
                    Console.WriteLine($"Summoner ranked Tier: {rankedData.Tier}");
                    Console.WriteLine($"Summoner ranked rank: {rankedData.Rank}");
                    Console.WriteLine($"Summoner ranked LP: {rankedData.LeaguePoints}");
                    Console.WriteLine($"Summoner rank Wins: {rankedData.Wins}");
                    Console.WriteLine($"Summoner rank Losses: {rankedData.Losses}\n");
                }
            }
            else
            {
                Console.WriteLine("Nenhuma informação de ranque encontrada para este invocador.");
            }

        }
    }
    static async Task<SummonerData> GetSummonerData(string apiKey, string summonerName, string region)
    {
        using (HttpClient client = new HttpClient())
        {
            string summonerApiUrl = $"https://{region}.api.riotgames.com/lol/summoner/v4/summoners/by-name/{summonerName}?api_key={apiKey}";

            HttpResponseMessage response = await client.GetAsync(summonerApiUrl);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                // Usando Newtonsoft.Json para desserializar o JSON
                SummonerData summonerData = JsonConvert.DeserializeObject<SummonerData>(responseBody);

                return summonerData;
            }
            else
            {
                Console.WriteLine($"Erro: {response.StatusCode} - {response.ReasonPhrase}");
                return null;
            }
        }
    }
    static async Task<List<RankedData>> GetRankedData(string apiKey, SummonerData summonerData, string region)
    {
        using (HttpClient client = new HttpClient())
        {
            string rankedApiUrl = $"https://{region}.api.riotgames.com/lol/league/v4/entries/by-summoner/{summonerData.Id}?api_key={apiKey}";

            HttpResponseMessage response = await client.GetAsync(rankedApiUrl);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                // Usando Newtonsoft.Json para desserializar o JSON
                List<RankedData> rankedDataList = JsonConvert.DeserializeObject<List<RankedData>>(responseBody);

                return rankedDataList;
            }
            else
            {
                Console.WriteLine($"Erro: {response.StatusCode} - {response.ReasonPhrase}");
                return null;
            }
        }
    }
    static async Task<List<String>> GetMatchId(string apiKey, SummonerData summonerData, string regionHistory)
    {
        using (HttpClient client = new HttpClient())
        {
            string matchListUrl = $"https://{regionHistory}.api.riotgames.com/lol/match/v5/matches/by-puuid/{summonerData.Puuid}/ids?api_key={apiKey}";

            HttpResponseMessage response = await client.GetAsync(matchListUrl);

            if (response.IsSuccessStatusCode)
            {

                // Usando Newtonsoft.Json para desserializar o JSON
                string responseBody = await response.Content.ReadAsStringAsync();
                // Use Newtonsoft.Json to deserialize the JSON
                List<string> matchIdList = JsonConvert.DeserializeObject<List<string>>(responseBody);

                return matchIdList;
            }
            else
            {
                Console.WriteLine($"Error retrieving match references: {response.StatusCode} - {response.ReasonPhrase}");
                return null;
            }
        }
    }
}


// Classe para representar os dados do invocador
public class SummonerData
{
    public string? Id { get; set; }
    public string? Puuid { get; set; }
    public string? Name { get; set; }
    public int ProfileIconId { get; set; }
    public long SummonerLevel { get; set; }
}
public class RankedData
{
    [JsonProperty("leagueId")]
    public string LeagueId { get; set; }

    [JsonProperty("queueType")]
    public string QueueType { get; set; }

    [JsonProperty("tier")]
    public string Tier { get; set; }

    [JsonProperty("rank")]
    public string Rank { get; set; }

    [JsonProperty("leaguePoints")]
    public int LeaguePoints { get; set; }

    [JsonProperty("wins")]
    public int Wins { get; set; }

    [JsonProperty("losses")]
    public int Losses { get; set; }
}