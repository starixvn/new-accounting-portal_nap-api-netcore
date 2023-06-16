FROM hub.nexdev.net/base/dotnet/core/sdk:7.0-alpine AS builder
WORKDIR /src
# Install the tools

COPY ./src/Nap.Auth/. ./src/Nap.Auth
COPY ./NAP.AuthWebApi/NAP.AuthWebApi.csproj ./NAP.AuthWebApi/NAP.AuthWebApi.csproj

WORKDIR /src/NAP.AuthWebApi
RUN dotnet nuget add source http://nuget.nexdev.net/api/v2
RUN dotnet restore -v diag

COPY ./NAP.AuthWebApi .

RUN dotnet publish -r linux-x64 --self-contained false -c Release -o /app 

FROM hub.nexdev.net/base/dotnet/core/aspnet:7.0-alpine AS base

WORKDIR /app

COPY --from=builder /app .

ENV TZ="America/La_Paz" 
    
# Don’t run as root
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app
USER dotnetuser
#


ENTRYPOINT ["dotnet", "NAP.AuthWebApi.dll"]