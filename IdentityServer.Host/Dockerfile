FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5101
# Add certificate to be able to communicate with other services via backchannel
COPY ["certificates/cacerts.crt", "/usr/local/share/ca-certificates/acme/"]
RUN update-ca-certificates

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["IdentityServer.Host/IdentityServer.Host.csproj", "IdentityServer.Host/"]
RUN dotnet restore "IdentityServer.Host/IdentityServer.Host.csproj"
COPY . .
WORKDIR "/src/IdentityServer.Host"
RUN dotnet build "./IdentityServer.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./IdentityServer.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityServer.Host.dll"]
