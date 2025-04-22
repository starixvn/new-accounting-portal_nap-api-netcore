FROM hub.nexdev.net/base/dotnet/core/sdk:9.0-alpine AS builder

WORKDIR /src
# Install the tools

COPY src/Nap.Auth/*.csproj ./src/Nap.Auth/
COPY NAP.AuthWebApi/*.csproj ./NAP.AuthWebApi/

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN --mount=type=cache,target=/root/.nuget/packages \
	dotnet restore -v n -r linux-x64 NAP.AuthWebApi/NAP.AuthWebApi.csproj

COPY src ./src
COPY NAP.AuthWebApi ./NAP.AuthWebApi

WORKDIR /src/NAP.AuthWebApi

RUN --mount=type=cache,target=/root/.nuget/packages \
	dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:9.0-alpine AS base

WORKDIR /app

COPY --from=builder /app .

ENV TZ="America/La_Paz" 
ENV COMPlus_EnableDiagnostics=0    

# Don’t run as root
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app && chown -R dotnetuser /tmp
USER dotnetuser

#

ENTRYPOINT ["dotnet", "NAP.AuthWebApi.dll"]
