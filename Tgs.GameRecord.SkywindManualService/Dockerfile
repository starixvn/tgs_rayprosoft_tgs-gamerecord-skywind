#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BuildConfiguration=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRecord.SkywindManualService/Tgs.GameRecord.SkywindManualService.csproj", "Tgs.GameRecord.SkywindManualService/"]
COPY ["Tgs.GameRecord.SkywindService.Core/Tgs.GameRecord.SkywindService.Core.csproj", "Tgs.GameRecord.SkywindService.Core/"]
RUN dotnet restore "Tgs.GameRecord.SkywindManualService/Tgs.GameRecord.SkywindManualService.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.SkywindManualService"
RUN dotnet build "Tgs.GameRecord.SkywindManualService.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.GameRecord.SkywindManualService.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.SkywindManualService.dll"]