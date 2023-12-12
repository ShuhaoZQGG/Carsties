using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
  .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(opt => 
  {
    opt.Authority = builder.Configuration["IdentityServiceUrl"];
    opt.RequireHttpsMetadata = false;
    opt.TokenValidationParameters.ValidateAudience = false;
    opt.TokenValidationParameters.NameClaimType = "username";
  });

builder.Services.AddCors(options => 
{
  options.AddPolicy("customPolicy", b => 
  {
    b.AllowAnyHeader()
      .AllowAnyMethod()
      .AllowCredentials()
      .WithOrigins(builder.Configuration["ClientApp"]);
  });
});

var app = builder.Build();
app.UseCors();
app.MapReverseProxy();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
