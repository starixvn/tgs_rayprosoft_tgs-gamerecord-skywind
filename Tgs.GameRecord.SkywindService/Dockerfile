#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BuildConfiguration=Release
RUN --mount=type=secret,id=nuget,dst=/pat cp /pat ~/.nuget/NuGet/NuGet.Config
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRecord.SkywindService/Tgs.GameRecord.SkywindService.csproj", "Tgs.GameRecord.SkywindService/"]
COPY ["Tgs.GameRecord.SkywindService.Core/Tgs.GameRecord.SkywindService.Core.csproj", "Tgs.GameRecord.SkywindService.Core/"]
RUN dotnet restore "Tgs.GameRecord.SkywindService/Tgs.GameRecord.SkywindService.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.SkywindService"
RUN dotnet build "Tgs.GameRecord.SkywindService.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.GameRecord.SkywindService.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.SkywindService.dll"]