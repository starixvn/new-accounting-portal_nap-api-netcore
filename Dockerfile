FROM registry.nexdev.net:5050/docker/dotnet/core/sdk:6.0 AS builder
WORKDIR /src
# Install the tools
RUN dotnet tool install --tool-path /tools dotnet-trace
RUN dotnet tool install --tool-path /tools dotnet-dump
RUN dotnet tool install --tool-path /tools dotnet-counters

COPY ./src/Nap.Auth/. ./src/Nap.Auth
COPY ./NAP.AuthWebApi/NAP.AuthWebApi.csproj ./NAP.AuthWebApi/NAP.AuthWebApi.csproj

WORKDIR /src/NAP.AuthWebApi
RUN dotnet nuget add source http://nuget.nexdev.net/api/v2
RUN dotnet restore -v diag

COPY ./NAP.AuthWebApi .

RUN dotnet publish -r linux-x64 --self-contained false -c Release -o /app 

FROM registry.nexdev.net:5050/docker/dotnet/core/aspnet:6.0 AS base

RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /usr/lib/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /usr/lib/ssl/openssl.cnf

WORKDIR /app
COPY --from=builder /app .
COPY --from=builder /tools /tools

# Don’t run as root
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app
USER dotnetuser
#


ENV TZ="America/La_Paz" 

ENTRYPOINT ["dotnet", "NAP.AuthWebApi.dll"]