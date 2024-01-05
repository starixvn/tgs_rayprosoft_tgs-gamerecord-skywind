#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BuildConfiguration=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRecord.Skywind/Tgs.GameRecord.Skywind.csproj", "Tgs.GameRecord.Skywind/"]
COPY ["Tgs.GameRecord.Skywind.Core/Tgs.GameRecord.Skywind.Core.csproj", "Tgs.GameRecord.Skywind.Core/"]
RUN dotnet restore "Tgs.GameRecord.Skywind/Tgs.GameRecord.Skywind.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.Skywind"
RUN dotnet build "Tgs.GameRecord.Skywind.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.GameRecord.Skywind.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.Skywind.dll"]