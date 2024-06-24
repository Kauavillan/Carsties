using MongoDB.Entities;

namespace SearchService;

public class AuctionSvcHttpClient
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;
    public AuctionSvcHttpClient(HttpClient httpClient, IConfiguration config){
        _config = config;
        _httpClient = httpClient;
    }

    public async Task<List<Item>> GetItemsForSearchDb(){
        var lastUpdate = await DB.Find<Item, string>()
            .Sort(x=>x.Descending(a=>a.UpdateAt))
            .Project(x=>x.UpdateAt.ToString())
            .ExecuteFirstAsync();
        Console.WriteLine(lastUpdate);
        return await _httpClient.GetFromJsonAsync<List<Item>>(_config["AuctionServiceUrl"]+"/api/auctions?date="+lastUpdate);
    }
}