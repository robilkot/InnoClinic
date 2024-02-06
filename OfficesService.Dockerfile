
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["OfficesService/OfficesService.csproj", "OfficesService/"]
COPY ["CommonData/InnoClinicCommonData.csproj", "CommonData/"]
RUN dotnet restore "OfficesService/OfficesService.csproj"
COPY . .
WORKDIR "/src/OfficesService"
RUN dotnet build "OfficesService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OfficesService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OfficesService.dll"]