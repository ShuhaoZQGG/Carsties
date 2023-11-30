using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;

namespace AuctionService.RequestHelpers;
public class MappingProfiles : Profile
{
  public MappingProfiles()
  {
    CreateMap<Auction, AuctionDto>().IncludeMembers(x => x.Item);
    CreateMap<Item, AuctionDto>();
    CreateMap<CreateAuctionDto, Auction>()
      .ForMember(destination => destination.Item, option => option.MapFrom(source => source));
    CreateMap<CreateAuctionDto, Item>();
    CreateMap<AuctionDto, AuctionCreated>();
    CreateMap<Auction, AuctionUpdated>().IncludeMembers(a => a.Item);
    CreateMap<Item, AuctionUpdated>();
  }
}
