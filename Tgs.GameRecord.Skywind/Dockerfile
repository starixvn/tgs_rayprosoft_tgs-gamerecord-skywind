#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRecord.Skywind/Tgs.GameRecord.Skywind.csproj", "Tgs.GameRecord.Skywind/"]
COPY ["Tgs.GameRecord.Skywind.Common/Tgs.GameRecord.Skywind.Common.csproj", "Tgs.GameRecord.Skywind.Common/"]
COPY ["Tgs.GameRecord.Skywind.Core/Tgs.GameRecord.Skywind.Core.csproj", "Tgs.GameRecord.Skywind.Core/"]
RUN dotnet restore "./Tgs.GameRecord.Skywind/./Tgs.GameRecord.Skywind.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.Skywind"
RUN dotnet build "./Tgs.GameRecord.Skywind.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.GameRecord.Skywind.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.Skywind.dll"]