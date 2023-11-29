using MongoDB.Entities;

namespace SearchService;
public class AuctionServiceHttpClient
{
  private readonly HttpClient _httpClient;
  private readonly IConfiguration _config;
  public AuctionServiceHttpClient(HttpClient httpClient, IConfiguration config)
  {
    _httpClient = httpClient;
    _config = config;
  }

  public async Task<List<Item>> GetItemsForSearchDb() 
  {
    var lastUpdatedItem = await DB.Find<Item, Item>()
        .Sort(x => x.Descending(x => x.UpdatedAt))
        .ExecuteFirstAsync();
    
    // Assuming Item class has an UpdatedAt property of type DateTime
    var lastUpdated = lastUpdatedItem?.UpdatedAt.ToString();

    return await _httpClient.GetFromJsonAsync<List<Item>>(_config["AuctionServiceUrl"] 
    + "/api/auctions?date=" + lastUpdated);
  }
}
