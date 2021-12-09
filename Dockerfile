FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["Dumplings.Cli/Dumplings.Cli.csproj", "Dumplings.Cli/"]
COPY ["Dumplings/Dumplings.csproj", "Dumplings/"]
RUN dotnet restore "Dumplings.Cli/Dumplings.Cli.csproj"
COPY . .
WORKDIR "/src/Dumplings.Cli"
RUN dotnet build "Dumplings.Cli.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dumplings.Cli.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY docker-entrypoint.sh docker-entrypoint.sh
ENTRYPOINT ["sh", "docker-entrypoint.sh"]
