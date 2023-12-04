using AuctionService.Controllers;
using AuctionService.DTOs;
using AuctionService.RequestHelpers;
using AutoFixture;
using AutoMapper;
using MassTransit;
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

    _controller = new AuctionController(_repo.Object, _mapper, _endpoint.Object);
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
}
