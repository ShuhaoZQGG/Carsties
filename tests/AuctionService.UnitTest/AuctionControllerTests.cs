using AuctionService.Controllers;
using AuctionService.DTOs;
using AuctionService.Entities;
using AuctionService.RequestHelpers;
using AutoFixture;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AuctionService.UnitTest;
public class AuctionControllerTests
{
  private readonly Mock<IAuctionRepository> _repo;
  private readonly Mock<IPublishEndpoint> _endpoint;
  private readonly IMapper _mapper;
  private readonly AuctionController _controller;
  private readonly Fixture _fixture;

  public AuctionControllerTests()
  {
    _fixture = new Fixture();
    _repo = new Mock<IAuctionRepository>();
    _endpoint = new Mock<IPublishEndpoint>();

    var mockMapper = new MapperConfiguration(mc => 
    {
      mc.AddMaps(typeof(MappingProfiles).Assembly);
    }).CreateMapper().ConfigurationProvider;
    _mapper = new Mapper(mockMapper);

    _controller = new AuctionController(_repo.Object, _mapper, _endpoint.Object)
    {
      ControllerContext = new ControllerContext
      {
        HttpContext = new DefaultHttpContext { User = Helpers.GetClaimsPrincipal() }
      }
    };
  }

  [Fact]
  public async Task GetAuctions_WithNoParams_Returns10Auctions() 
  {
    // Arrange
    var auctions = _fixture.CreateMany<AuctionDto>(10).ToList();
    _repo.Setup(repo => repo.GetAuctionsAsync(null)).ReturnsAsync(auctions);

    // Act
    var result = await _controller.GetAllAuctions(null);

    // Assert
    Assert.Equal(10, result.Value.Count);
    Assert.IsType<ActionResult<List<AuctionDto>>>(result);
  }

  [Fact]
  public async Task GetAuctionById_WithValidGuid_ReturnsAuction()
  {
    // Arrange
    var auction = _fixture.Create<AuctionDto>();
    _repo.Setup(repo => repo.GetAuctionByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);

    // Act
    var result = await _controller.GetAuctionById(auction.Id);

    // Assert
    Assert.Equal(auction.Make, result.Value.Make);
    Assert.IsType<ActionResult<AuctionDto>>(result);
  }

  [Fact]
  public async Task GetAuctionById_WithInValidGuid_ReturnsNotFound()
  {
    // Arrange
    _repo.Setup(repo => repo.GetAuctionByIdAsync(It.IsAny<Guid>())).ReturnsAsync(value: null);

    // Act
    var result = await _controller.GetAuctionById(Guid.NewGuid());

    // Assert
    Assert.IsType<NotFoundResult>(result.Result);
  }

  [Fact]
  public async Task CreateAuction_WithInValidCreateAuctionDto_ReturnsCreatedAtAction()
  {
    // Arrange
    var auction = _fixture.Create<CreateAuctionDto>();
    _repo.Setup(repo => repo.AddAuction(It.IsAny<Auction>()));
    _repo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);
    // Act
    var result = await _controller.CreateAuction(auction);
    var createdResult = result.Result as CreatedAtActionResult;
    // Assert
    Assert.NotNull(createdResult);
    Assert.Equal("GetAuctionById", createdResult.ActionName);
    Assert.IsType<AuctionDto>(createdResult.Value);
  }

  [Fact]
  public async Task CreateAuction_WithResultFalse_ReturnsBadRequestObjectResult() 
  {
    // Arrange
    var auction = _fixture.Create<CreateAuctionDto>();
    _repo.Setup(repo => repo.AddAuction(It.IsAny<Auction>()));
    _repo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(false);

    // Act
    var result = await _controller.CreateAuction(auction);
    var badRequestResult = result.Result as BadRequestObjectResult;

    // Assert
    Assert.NotNull(badRequestResult);
    Assert.IsType<BadRequestObjectResult>(badRequestResult);  
  }
  
  [Fact]
  public async Task UpdateAuction_WithInValidId_ReturnsNotFoundResult()
  {
    // Arrange
    var auction = _fixture.Create<UpdateAuctionDto>();
    _repo.Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(value: null);
    
    // Act
    var result = await _controller.UpdateAuction(Guid.NewGuid(), auction);
    
    // Assert
    Assert.IsType<NotFoundResult>(result);
  }
  
  [Fact]
  public async Task UpdateAuction_WithSellerNull_ReturnsForbidden() 
  {
    // Arrange
    var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
    var updatedAuction = _fixture.Create<UpdateAuctionDto>();
    _repo.Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);
    _repo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

    // Act
    var result = await _controller.UpdateAuction(auction.Id, updatedAuction);
    var forbidResult = result as ForbidResult;

    // Assert
    Assert.NotNull(forbidResult);
    Assert.IsType<ForbidResult>(forbidResult);  
  }

  [Fact]
  public async Task UpdateAuction_WithResultFalse_ReturnsBadRequestObjectResult() 
  {
    // Arrange
    var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
    var item = _fixture.Build<Item>().Without(x => x.Auction).Create();
    auction.Item = item;
    var updatedAuction = _fixture.Create<UpdateAuctionDto>();
    auction.Seller = "test";
    _repo.Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);
    _repo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(false);

    // Act
    var result = await _controller.UpdateAuction(auction.Id, updatedAuction);
    var badRequestObjectResult = result as BadRequestObjectResult;

    // Assert
    Assert.NotNull(badRequestObjectResult);
    Assert.IsType<BadRequestObjectResult>(badRequestObjectResult);  
  }

  [Fact]
  public async Task UpdateAuction_WithValidIdAndUpdateAuctionDto_ReturnsOk() 
  {
    // Arrange
    var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
    var item = _fixture.Build<Item>().Without(x => x.Auction).Create();
    auction.Item = item;
    var updatedAuction = _fixture.Create<UpdateAuctionDto>();
    auction.Seller = "test";
    _repo.Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);
    _repo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

    // Act
    var result = await _controller.UpdateAuction(auction.Id, updatedAuction);

    // Assert
    Assert.NotNull(result);
    Assert.IsType<OkResult>(result);  
  }

  [Fact]
  public async Task DeleteAuction_WithInvalidId_ReturnsNotFound()
  {
    // Arrange
    var auction = _fixture.Create<UpdateAuctionDto>();
    _repo.Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(value: null);
    
    // Act
    var result = await _controller.DeleteAuction(Guid.NewGuid());
    
    // Assert
    Assert.IsType<NotFoundResult>(result);
  }


  [Fact]
  public async Task DeleteAuction_WithResultFalse_ReturnsBadRequest() 
  {
    // Arrange
    var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
    auction.Seller = "test";
    _repo.Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);
    _repo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(false);

    // Act
    var result = await _controller.DeleteAuction(auction.Id);
    var badRequestObjectResult = result as BadRequestObjectResult;

    // Assert
    Assert.NotNull(badRequestObjectResult);
    Assert.IsType<BadRequestObjectResult>(badRequestObjectResult);  
  } 

  [Fact]
  public async Task RemoveAuction_WithSellerNull_ReturnsForbidden() 
  {
    // Arrange
    var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
    _repo.Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);
    _repo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

    // Act
    var result = await _controller.DeleteAuction(auction.Id);
    var forbidResult = result as ForbidResult;

    // Assert
    Assert.NotNull(forbidResult);
    Assert.IsType<ForbidResult>(forbidResult);  
  }

  [Fact]
  public async Task DeleteAuction_WithValidIdAndAuctionDto_ReturnsOk() 
  {
    // Arrange
    var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
    auction.Seller = "test";
    _repo.Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);
    _repo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

    // Act
    var result = await _controller.DeleteAuction(auction.Id);

    // Assert
    Assert.NotNull(result);
    Assert.IsType<OkResult>(result);  
  }
}
