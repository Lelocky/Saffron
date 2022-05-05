#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Spice.Saffron/Spice.Saffron.csproj", "Spice.Saffron/"]
RUN dotnet restore "Spice.Saffron/Spice.Saffron.csproj"
COPY . .
WORKDIR "/src/Spice.Saffron"
RUN dotnet build "Spice.Saffron.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Spice.Saffron.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Spice.Saffron.dll"]