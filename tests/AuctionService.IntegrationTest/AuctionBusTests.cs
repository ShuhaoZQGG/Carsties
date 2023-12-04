using System.Net;
using System.Net.Http.Json;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.IntegrationTests;
using Contracts;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionService.IntegrationTest;
[Collection("Shared collection")]
public class AuctionBusTests : IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private readonly HttpClient _httpClient;
    private ITestHarness _testHarness;
    private const string _gT_ID = "afbee524-5972-4075-8800-7d1f9d7b0a0c";

    public AuctionBusTests(CustomWebAppFactory factory)
    {
        _factory = factory;
        _testHarness = factory.Services.GetTestHarness();
        _httpClient = factory.CreateClient();
    }
    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();
        DbHelper.ReinitDbForTests(db);
        return Task.CompletedTask;
    }

    private static CreateAuctionDto GetAuctionForCreate()
    {
        return new CreateAuctionDto
        {
            Make = "test",
            Model = "testModel",
            ImageUrl = "test",
            Color = "test",
            Mileage = 10,
            Year = 10,
            ReservePrice = 10
        };
    }

    [Fact]
    public async Task CreateAuction_WithValidObject_ShouldPublishAuctionCreated()
    {
      // Arrange
      var auction = GetAuctionForCreate();
      _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));
      
      // Act
      var response = await _httpClient.PostAsJsonAsync("api/auctions", auction);

      // Assert
      response.EnsureSuccessStatusCode();
      Assert.True(await _testHarness.Published.Any<AuctionCreated>());
    }
}
