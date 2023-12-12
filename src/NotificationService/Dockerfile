FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app
EXPOSE 80

# copy all .csproj files and restore as distinct layers. 
# Use of the same COPY command for every dockerfile
# in the project to take advantages of docker caching.

COPY Carsties.sln Carsties.sln
# wildcard doesn't work in docker (for now) so we need to manually copy everything
COPY src/AuctionService/AuctionService.csproj src/AuctionService/AuctionService.csproj
COPY src/SearchService/SearchService.csproj src/SearchService/SearchService.csproj
COPY src/GatewayService/GatewayService.csproj src/GatewayService/GatewayService.csproj
COPY src/Contracts/Contracts.csproj src/Contracts/Contracts.csproj
COPY src/IdentityService/IdentityService.csproj src/IdentityService/IdentityService.csproj
COPY src/BiddingService/BiddingService.csproj src/BiddingService/BiddingService.csproj
COPY src/NotificationService/NotificationService.csproj src/NotificationService/NotificationService.csproj
COPY tests/AuctionService.UnitTest/AuctionService.UnitTest.csproj tests/AuctionService.UnitTest/AuctionService.UnitTest.csproj
COPY tests/AuctionService.IntegrationTest/AuctionService.IntegrationTest.csproj tests/AuctionService.IntegrationTest/AuctionService.IntegrationTest.csproj
# Restore package deps
RUN dotnet restore Carsties.sln

# Copy the app folders over
COPY src/NotificationService src/NotificationService
COPY src/Contracts src/Contracts
WORKDIR /app/src/NotificationService
RUN dotnet publish -c Release -o /app/src/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/src/out .
ENTRYPOINT [ "dotnet", "NotificationService.dll" ]