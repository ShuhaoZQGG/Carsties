using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace BiddingService;
public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
{
  public async Task Consume(ConsumeContext<AuctionCreated> context)
  {
    var auciton = new Auction 
    {
      ID = context.Message.Id.ToString(),
      Seller = context.Message.Seller,
      AuctionEnd = context.Message.AuctionEnd,
      ReservePrice = context.Message.ReservePrice
    };

    await auciton.SaveAsync();
  }
}
