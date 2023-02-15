# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# copy everything and restore
COPY . .
RUN dotnet restore ./Presentation/ITMO.Dev.ASAP.WebApi

# copy everything else and build app
RUN dotnet publish -c release -o /app --no-restore Presentation/ITMO.Dev.ASAP.WebApi

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app ./
ENV ASPNETCORE_URLS=http://0.0.0.0:5069 \
    ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "ITMO.Dev.ASAP.WebApi.dll"]
