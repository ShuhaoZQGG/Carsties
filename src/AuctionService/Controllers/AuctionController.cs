using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionController : ControllerBase
{
  private readonly AuctionDbContext _context;
  private readonly IMapper _mapper;
  private readonly IPublishEndpoint _publishEndpoint;

  public AuctionController(AuctionDbContext context, IMapper mapper, IPublishEndpoint publishEndpoint)
  {
    _context = context;
    _mapper = mapper;
    _publishEndpoint = publishEndpoint;
  }

  [HttpGet]
  public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string date) 
  {
    var query = _context.Auctions.OrderBy(x => x.Item.Make).AsQueryable();

    if (!string.IsNullOrEmpty(date)) 
    {
      query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
    }

    return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id) 
  {
    var auction = await _context.Auctions
      .Include(a => a.Item)
      .FirstOrDefaultAsync(a => a.Id == id);

    return _mapper.Map<AuctionDto>(auction);
  }

  [Authorize]
  [HttpPost]
  public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto) 
  {
    var auction = _mapper.Map<Auction>(auctionDto);
    auction.Seller = User.Identity.Name;
    _context.Auctions.Add(auction);
    var result = await _context.SaveChangesAsync() > 0;

    var newAuction = _mapper.Map<AuctionDto>(auction);
    Console.WriteLine("publishing " + newAuction.Id);
    await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));

    if (!result) return BadRequest("Could not save the changes to the DB");
    
    return CreatedAtAction(nameof(GetAuctionById), new { auction.Id }, _mapper.Map<AuctionDto>(auction));
  }
  
  [Authorize]
  [HttpPut("{id}")]
  public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto auctionDto) 
  {
    var auction = await _context.Auctions.Include(x => x.Item)
      .FirstOrDefaultAsync(x => x.Id == id);

    if (auction == null) return NotFound();

    if (auction.Seller != User.Identity.Name) return Forbid();

    auction.Item.Make = auctionDto.Make ?? auction.Item.Make;
    auction.Item.Model = auctionDto.Model ?? auction.Item.Model;
    auction.Item.Color = auctionDto.Color ?? auction.Item.Color;
    auction.Item.Mileage = auctionDto.Mileage ?? auction.Item.Mileage;
    auction.Item.Year = auctionDto.Year ?? auction.Item.Year;

    await _publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));

    var result = await _context.SaveChangesAsync() > 0;

    if (!result) return BadRequest("Problem Saving Changes");

    return Ok();
  }

  [Authorize]
  [HttpDelete("{id}")]
  public async Task<ActionResult> DeleteAuction(Guid id) 
  {
    var auction = await _context.Auctions.FindAsync(id);

    if (auction == null) return NotFound();

    if (auction.Seller != User.Identity.Name) return Forbid();

    _context.Auctions.Remove(auction);

    await _publishEndpoint.Publish<AuctionDeleted>(new { Id = auction.Id.ToString() });

    var result = await _context.SaveChangesAsync() > 0;

    if (!result) return BadRequest("Could not update DB");

    return Ok();
  }
}
