FROM hub.nexdev.net/base/dotnet/core/sdk:8.0-alpine AS builder

WORKDIR "/src"
COPY ["/src/Aff12B.Common.DataDomain", "./Aff12B.Common.DataDomain"]
COPY ["/src/Aff12B.Common.Infrastructure", "./Aff12B.Common.Infrastructure"]
COPY ["/src/Aff12B.CronJobs.Core", "./Aff12B.CronJobs.Core"]
COPY ["/src/Aff12B.CronJobs.Infrastructure", "./Aff12B.CronJobs.Infrastructure"]
COPY ["/src/Aff12B.CronJobs.Host/Aff12B.CronJobs.Host.csproj", "./Aff12B.CronJobs.Host/Aff12B.CronJobs.Host.csproj"]

# Restore packages
WORKDIR "/src/Aff12B.CronJobs.Host"
RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore -v n -r linux-musl-x64 "./Aff12B.CronJobs.Host.csproj"

COPY "/src/Aff12B.CronJobs.Host" .

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish -r linux-musl-x64 --self-contained false -o /app/publish

FROM hub.nexdev.net/base/dotnet/core/aspnet:8.0-alpine

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

# Don’t run as root
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app && chown -R dotnetuser /tmp
USER dotnetuser
#

WORKDIR "/app"

COPY --from=builder /app/publish .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Aff12B.CronJobs.Host.dll"]
