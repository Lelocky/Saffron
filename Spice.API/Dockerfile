#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Spice.API/Spice.API.csproj", "Spice.API/"]
RUN dotnet restore "Spice.API/Spice.API.csproj"
COPY . .
WORKDIR "/src/Spice.API"
RUN dotnet build "Spice.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Spice.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Spice.API.dll"]