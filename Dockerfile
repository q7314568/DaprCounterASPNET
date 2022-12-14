#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5300

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DaprCounterASPNET.csproj", "."]
RUN dotnet restore "./DaprCounterASPNET.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "DaprCounterASPNET.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DaprCounterASPNET.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DaprCounterASPNET.dll"]