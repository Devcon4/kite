FROM mcr.microsoft.com/dotnet/nightly/sdk:9.0-noble-aot AS api

ARG VERSION=1.0.0
ARG APP_UID=1654

# Install native build tools required for NativeAOT
# RUN apt-get update && \
# 	apt-get install -y clang zlib1g-dev && \
# 	rm -rf /var/lib/apt/lists/*

WORKDIR /build

COPY api/*.csproj ./

RUN dotnet restore ./ -r linux-x64

COPY api/ ./

RUN dotnet publish -c Release --no-restore -o /build/publish /p:Version=${VERSION} -r linux-x64 ./
RUN ls -la /build/publish

FROM node:24-alpine AS app
WORKDIR /build

COPY app/package.json ./
COPY app/package-lock.json ./

RUN npm ci;

COPY app/ ./

RUN npm run build;

# FROM node:24-alpine AS user-setup
# ARG APP_UID=1654
# RUN addgroup -g ${APP_UID} appgroup && adduser -u ${APP_UID} -D appuser -G appgroup

FROM  mcr.microsoft.com/dotnet/nightly/aspnet:9.0-noble-chiseled AS final
USER $APP_UID
WORKDIR /app

# Copy application files
COPY --chown=${APP_UID}:${APP_UID} --from=api /build/publish ./
COPY --chown=${APP_UID}:${APP_UID} --from=app /build/dist/kite/browser ./wwwroot/
# Have to use higher port due to non-root user
ENV ASPNETCORE_URLS="http://*:8080"

EXPOSE 8080
ENTRYPOINT ["/app/api"]
