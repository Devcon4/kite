FROM mcr.microsoft.com/dotnet/sdk:6.0 as api

ARG VERSION=1.0.0

WORKDIR /build

COPY api/*.csproj ./

RUN dotnet restore ./

COPY api/ ./

RUN dotnet publish -c Release --no-restore -o /build/publish /p:Version=${VERSION} ./

FROM node:16-alpine as app
WORKDIR /build

COPY app/package.json ./
COPY app/package-lock.json ./

RUN npm ci;

COPY app/ ./

RUN npm run build;

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as entry
ARG USER=appuser
ARG UID=1001
ARG GID=1001

RUN groupadd -g $GID $USER
RUN useradd --uid $UID --gid $GID --create-home --shell /bin/bash $USER
RUN mkdir /app && chown -R $USER:$USER /app

USER $USER

WORKDIR /app
COPY --chown=$USER:$USER --from=api /build/publish .
COPY --chown=$USER:$USER --from=app /build/dist/kite ./wwwroot/

# Have to use higher port due to non-root user
ENV ASPNETCORE_URLS="http://*:8080"

EXPOSE 8080
ENTRYPOINT ["dotnet", "api.dll"]
