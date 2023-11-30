using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace SearchService;

// MassTransit is convention based, it expects our consumer ended with the work 'consumer'
public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
{
  private readonly IMapper _mapper;

  public AuctionCreatedConsumer(IMapper mapper)
  {
    _mapper = mapper;
  }
  public async Task Consume(ConsumeContext<AuctionCreated> context)
  {
    Console.WriteLine("--> Consuming Auction Created: " + context.Message.Id);

    var item = _mapper.Map<Item>(context.Message);

    if (item.Model == "Foo") throw new ArgumentException("Cannot sell cars with name of Foo");

    await item.SaveAsync();
  }
}
