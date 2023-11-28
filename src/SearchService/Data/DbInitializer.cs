using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
namespace SearchService;
public class DbInitializer
{
  public static async Task InitDb(WebApplication app) 
  {
    await DB.InitAsync("SearchDB", MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));
    await DB.Index<Item>()
      .Key(x => x.Make, KeyType.Text)
      .Key(x => x.Model, KeyType.Text)
      .Key(x => x.Color, KeyType.Text)
      .CreateAsync();

    var count = await DB.CountAsync<Item>();

    if (count == 0) 
    {
      Console.WriteLine("No Data - Will attempt to seed");
      SeedData();
    }
  }

  private static async void SeedData()
  {
    var itemData = await File.ReadAllTextAsync("Data/auctions.json");

    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

    var items = JsonSerializer.Deserialize<List<Item>>(itemData, options);

    await DB.SaveAsync(items);
  }
}
