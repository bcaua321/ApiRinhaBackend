FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/nightly/sdk:8.0-jammy-aot AS build
ARG TARGETARCH
WORKDIR /source
EXPOSE 80

# copy csproj and restore as distinct layers
COPY "api.csproj" .
COPY "nuget.config" .
RUN dotnet restore -r linux-$TARGETARCH

# copy and publish app and libraries
COPY . .
RUN dotnet publish -r linux-$TARGETARCH  -o /app
RUN rm /app/*.dbg /app/*.Development.json

# final stage/image
FROM mcr.microsoft.com/dotnet/nightly/runtime-deps:8.0-jammy-chiseled-aot
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
ENTRYPOINT ["./api"]