
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ProfilesService/ProfilesService.csproj", "ProfilesService/"]
COPY ["CommonData/CommonData.csproj", "CommonData/"]
RUN dotnet restore "ProfilesService/ProfilesService.csproj"
COPY . .
WORKDIR "/src/ProfilesService"
RUN dotnet build "ProfilesService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProfilesService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProfilesService.dll"]