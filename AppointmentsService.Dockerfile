
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5004

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["AppointmentsService/AppointmentsService.csproj", "AppointmentsService/"]
COPY ["CommonData/CommonData.csproj", "CommonData/"]
RUN dotnet restore "AppointmentsService/AppointmentsService.csproj"
COPY . .
WORKDIR "/src/AppointmentsService"
RUN dotnet build "AppointmentsService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AppointmentsService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AppointmentsService.dll"]