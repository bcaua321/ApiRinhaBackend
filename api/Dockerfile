FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build
RUN apt-get update && apt-get -qq install clang zlib1g-dev
WORKDIR /source
EXPOSE 80

# copy csproj and restore as distinct layers
COPY "api.csproj" .
COPY "nuget.config" .
RUN dotnet restore -r linux-x64

# copy and publish app and libraries
COPY . .
RUN dotnet publish -r linux-x64  -o /app
RUN rm /app/*.dbg /app/*.Development.json

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:8.0-jammy
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
ENTRYPOINT ["./api"]