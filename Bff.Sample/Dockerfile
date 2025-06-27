FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5102
# Add certificate to be able to communicate with other services via backchannel
COPY ["certificates/cacerts.crt", "/usr/local/share/ca-certificates/acme/"]
RUN update-ca-certificates

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Bff.Sample/Bff.Sample.csproj", "Bff.Sample/"]
COPY ["Bff.Sample.Client/Bff.Sample.Client.csproj", "Bff.Sample.Client/"]
RUN dotnet restore "Bff.Sample/Bff.Sample.csproj"
COPY . .
WORKDIR "/src/Bff.Sample"
RUN dotnet build "./Bff.Sample.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Bff.Sample.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bff.Sample.dll"]
