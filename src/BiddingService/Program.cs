using BiddingService;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MongoDB.Driver;
using MongoDB.Entities;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("bids", false));

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.UseMessageRetry(r => {
          r.Handle<RabbitMqConnectionException>();
          r.Interval(5, TimeSpan.FromSeconds(10));
        });
        cfg.Host(builder.Configuration["RabbitMq:Host"], "/", host => 
        {
          host.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest"));
          host.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest"));
        });
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(opt => 
  {
    opt.Authority = builder.Configuration["IdentityServiceUrl"];
    opt.RequireHttpsMetadata = false;
    opt.TokenValidationParameters.ValidateAudience = false;
    opt.TokenValidationParameters.NameClaimType = "username";
  });

  builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
  builder.Services.AddHostedService<CheckAuctionFinished>();
  builder.Services.AddScoped<GrpcAuctionClient>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await Policy
  .Handle<TimeoutException>()
  .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(10))
  .ExecuteAndCaptureAsync(async () => 
    await DB.InitAsync("BidDb", 
      MongoClientSettings.FromConnectionString(builder.Configuration.GetConnectionString("BidDbConnection")))
  );

app.Run();
