using MongoDB.Entities;

namespace BiddingService;
public class AuctionEntity : Entity
{
  public DateTime AuctionEnd { get; set; }
  public string Seller { get; set; }
  public int ReservePrice { get; set; }
  public bool Finished { get; set; }  
}
