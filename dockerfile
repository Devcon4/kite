FROM mcr.microsoft.com/dotnet/sdk:9.0 as api

ARG VERSION=1.0.0

WORKDIR /build

COPY api/*.csproj ./

RUN dotnet restore ./

COPY api/ ./

RUN dotnet publish -c Release --no-restore -o /build/publish /p:Version=${VERSION} ./

FROM node:24-alpine as app
WORKDIR /build

COPY app/package.json ./
COPY app/package-lock.json ./

RUN npm ci;

COPY app/ ./

RUN npm run build;

FROM mcr.microsoft.com/dotnet/aspnet:9.0 as entry
USER $APP_UID

WORKDIR /app
COPY --chown=$APP_UID:$APP_UID --from=api /build/publish .
COPY --chown=$APP_UID:$APP_UID --from=app /build/dist/kite/browser ./wwwroot/

# Have to use higher port due to non-root user
ENV ASPNETCORE_URLS="http://*:8080"

EXPOSE 8080
ENTRYPOINT ["dotnet", "api.dll"]
