
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DocumentsService/DocumentsService.csproj", "DocumentsService/"]
COPY ["CommonData/InnoClinicCommonData.csproj", "CommonData/"]
RUN dotnet restore "DocumentsService/DocumentsService.csproj"
COPY . .
WORKDIR "/src/DocumentsService"
RUN dotnet build "DocumentsService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DocumentsService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DocumentsService.dll"]