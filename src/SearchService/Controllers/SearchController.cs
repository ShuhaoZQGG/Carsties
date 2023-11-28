using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;

namespace SearchService;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
  [HttpGet]
  public async Task<ActionResult<List<Item>>> SearchItems([FromQuery] SearchParams searchParams) 
  {
    var searchTerm = searchParams.searchTearm;
    var pageNumber = searchParams.PageNumber;
    var pageSize = searchParams.PageSize;
    var winner = searchParams.Winner;
    var seller = searchParams.Seller;
    var orderby = searchParams.OrderBy;
    var filterby = searchParams.FilterBy;

    var query = DB.PagedSearch<Item, Item>();
    query.Sort(x => x.Ascending(a => a.Make));
    if (!string.IsNullOrEmpty(searchTerm)) 
    {
      query.Match(Search.Full, searchTerm).SortByTextScore();
    }

    query = orderby switch
    {
      "make" => query.Sort(x => x.Ascending(x => x.Make)),
      "new" => query.Sort(x => x.Descending(x => x.CreatedAt)),
      // Default
      _ => query.Sort(x => x.Ascending(a => a.AuctionEnd)),
    };

    query = filterby switch
    {
      "finished" => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
      "endingSoon" => query.Match(x => x.AuctionEnd < DateTime.UtcNow.AddHours(6) 
        && x.AuctionEnd > DateTime.UtcNow),
      _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow)
    };

    if (!string.IsNullOrEmpty(seller)) 
    {
      query.Match(x => x.Seller == seller);
    }

    if (!string.IsNullOrEmpty(winner)) 
    {
      query.Match(x => x.Seller == winner);
    }


    query.PageNumber(pageNumber);
    query.PageSize(pageSize);
    var result = await query.ExecuteAsync();
    return Ok(new 
    {
      results = result.Results,
      pageNumber = pageNumber,
      pageSize = pageSize,
      pageCount = result.PageCount,
      totalCount = result.TotalCount
    });
  }
}
