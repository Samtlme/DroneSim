# base
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["DroneSim.Api/DroneSim.Api.csproj", "DroneSim.Api/"]
COPY ["DroneSim.Application/DroneSim.Application.csproj", "DroneSim.Application/"]
COPY ["DroneSim.Core/DroneSim.Core.csproj", "DroneSim.Core/"]
COPY ["DroneSim.Infrastructure/DroneSim.Infrastructure.csproj", "DroneSim.Infrastructure/"]

RUN dotnet restore "DroneSim.Api/DroneSim.Api.csproj"

COPY . .
WORKDIR "/src/DroneSim.Api"
RUN dotnet publish "DroneSim.Api.csproj" -c Release -o /app/publish

# final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DroneSim.Api.dll"]
