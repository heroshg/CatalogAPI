FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

COPY ["CatalogAPI.sln", "."]
COPY ["src/Catalog.Domain/Catalog.Domain.csproj", "src/Catalog.Domain/"]
COPY ["src/Catalog.Application/Catalog.Application.csproj", "src/Catalog.Application/"]
COPY ["src/Catalog.Infrastructure/Catalog.Infrastructure.csproj", "src/Catalog.Infrastructure/"]
COPY ["src/Catalog.API/Catalog.API.csproj", "src/Catalog.API/"]
RUN dotnet restore "src/Catalog.API/Catalog.API.csproj"

COPY . .
RUN dotnet publish "src/Catalog.API/Catalog.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
RUN apk upgrade --no-cache
WORKDIR /app
COPY --from=build /app/publish .
USER app
ENTRYPOINT ["dotnet", "Catalog.API.dll"]
