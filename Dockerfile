FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ["src/Host/Host.csproj", "src/Host/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]
COPY ["src/Client/Client.csproj", "src/Client/"]
COPY ["src/Client.Infrastructure/Client.Infrastructure.csproj", "src/Client.Infrastructure/"]

RUN dotnet restore "src/Host/Host.csproj"

COPY . .
WORKDIR "/src/src/Host"

RUN dotnet publish "Host.csproj" -c Release --no-restore -o /app/publish

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FSH.BlazorWebAssembly.Host.dll"]