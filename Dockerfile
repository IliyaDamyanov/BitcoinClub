# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY BitcoinClub/BitcoinClub.csproj BitcoinClub/
RUN dotnet restore BitcoinClub/BitcoinClub.csproj

COPY BitcoinClub/ BitcoinClub/
WORKDIR /src/BitcoinClub
RUN dotnet publish -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

RUN adduser --disabled-password --gecos '' appuser

COPY --from=build /app/publish .

RUN mkdir -p /app/logs && chown -R appuser:appuser /app

USER appuser

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD curl -f http://localhost:8080/ || exit 1

ENTRYPOINT ["dotnet", "BitcoinClub.dll"]
