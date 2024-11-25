ARG CI_REGISTRY_IMAGE

FROM $CI_REGISTRY_IMAGE:builder AS deps

RUN mkdir -p /src/packages

FROM hub.nexdev.net/base/dotnet/core/sdk:8.0-alpine AS builder

ENV NUGET_PACKAGES=/src/packages

COPY --from=deps $NUGET_PACKAGES/. $NUGET_PACKAGES/

WORKDIR "/src"

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2

COPY ["src/Aff12B.Common.DataDomain/Aff12B.Common.DataDomain.csproj", "./Aff12B.Common.DataDomain/"]
RUN dotnet restore -v n -r linux-x64 ./Aff12B.Common.DataDomain/Aff12B.Common.DataDomain.csproj

COPY ["src/Aff12B.Ext.API.Core/Aff12B.Ext.API.Core.csproj", "./Aff12B.Ext.API.Core/"]
RUN dotnet restore -v n -r linux-x64 ./Aff12B.Ext.API.Core/Aff12B.Ext.API.Core.csproj

COPY ["src/Aff12B.Ext.API.Infastructure/Aff12B.Ext.API.Infastructure.csproj", "./Aff12B.Ext.API.Infastructure/"]
RUN dotnet restore -v n -r linux-x64 ./Aff12B.Ext.API.Infastructure/Aff12B.Ext.API.Infastructure.csproj

COPY ["src/Aff12B.Ext.API.Site/Aff12B.Ext.API.Site.csproj", "./Aff12B.Ext.API.Site/"]
RUN dotnet restore -v n -r linux-x64 ./Aff12B.Ext.API.Site/Aff12B.Ext.API.Site.csproj

COPY "/src" .

WORKDIR "/src/Aff12B.Ext.API.Site"
RUN dotnet publish -r linux-x64 --self-contained false -o /app/publish

FROM hub.nexdev.net/base/dotnet/core/aspnet:8.0-alpine

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

# Don't run as root
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app && chown -R dotnetuser /tmp
USER dotnetuser
#

WORKDIR "/app"

COPY --from=builder /app/publish .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Aff12B.Ext.API.Site.dll"]
