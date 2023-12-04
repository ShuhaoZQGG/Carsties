using AuctionService.Data;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace AuctionService.IntegrationTest;
public class CustomWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
  private PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder().Build();
  public async Task InitializeAsync()
  {
    await _postgreSqlContainer.StartAsync();
  }

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    // Remove AuctionDbContext in AuctionService, and replace with the test one.
    builder.ConfigureTestServices(services => 
    {
      services.RemoveContext<AuctionDbContext>();


      services.AddDbContext<AuctionDbContext>(options => 
      {
        options.UseNpgsql(_postgreSqlContainer.GetConnectionString());
      });
      services.AddMassTransitTestHarness();
      services.EnsureCreated<AuctionDbContext>();
    });
    base.ConfigureWebHost(builder);
  }

  Task IAsyncLifetime.DisposeAsync() => _postgreSqlContainer.DisposeAsync().AsTask();
}
