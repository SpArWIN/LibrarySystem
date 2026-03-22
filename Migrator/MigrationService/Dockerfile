FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY Directory.Build.props .

COPY ["Migrator/MigrationService/MigrationService.csproj", "Migrator/MigrationService/"]
COPY ["Common/Common.Migrator/Common.Migrator.csproj", "Common/Common.Migrator/"]
COPY ["Core/Core.Infrastructure/Core.Infrastructure.csproj", "Core/Core.Infrastructure/"]
COPY ["Common/Common.Contracts/Common.Contracts.csproj", "Common/Common.Contracts/"]
COPY ["Common/Common.Db/Common.Db.csproj", "Common/Common.Db/"]
COPY ["Common/Common.Policies/Common.Policies.csproj", "Common/Common.Policies/"]
COPY ["Common/Common.Extensions/Common.Extensions.csproj", "Common/Common.Extensions/"]
COPY ["Common/Common.Http/Common.Http.csproj", "Common/Common.Http/"]
COPY ["Common/Common.Logging/Common.Logging.csproj", "Common/Common.Logging/"]
COPY ["Core/Core.Domain/Core.Domain.csproj", "Core/Core.Domain/"]

RUN dotnet restore "Migrator/MigrationService/MigrationService.csproj" -r linux-x64
COPY . .

WORKDIR "/src/Migrator/MigrationService"
RUN dotnet publish "MigrationService.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    --self-contained true \
    -r linux-x64 \
    /p:PublishSingleFile=true \
    /p:PublishTrimmed=true \
    /p:TrimMode=full \
    /p:DebugType=none \
    /p:DebugSymbols=false \
    /p:EnableCompressionInSingleFile=true

FROM mcr.microsoft.com/dotnet/runtime-deps:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["./MigrationService"]
