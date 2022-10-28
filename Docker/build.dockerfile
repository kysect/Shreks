# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY ./Presentation/Kysect.Shreks.WebApi/Kysect.Shreks.WebApi.csproj ./Kysect.Shreks.WebApi/Kysect.Shreks.WebApi.csproj
COPY ./Presentation/Kysect.Shreks.Controllers/Kysect.Shreks.Controllers.csproj ./Presentation/Kysect.Shreks.Controllers/Kysect.Shreks.Controllers.csproj
COPY ./Presentation/Kysect.Shreks.Sdk/Kysect.Shreks.Sdk.csproj ./Presentation/Kysect.Shreks.Sdk/Kysect.Shreks.Sdk.csproj
COPY ./Presentation/Kysect.Shreks.Sdk/Kysect.Shreks.AdminPanel.csproj ./Presentation/Kysect.Shreks.Sdk/Kysect.Shreks.AdminPanel.csproj
COPY ./Application/Kysect.Shreks.Application.GithubWorkflow/Kysect.Shreks.Application.GithubWorkflow.csproj ./Application/Kysect.Shreks.Application.GithubWorkflow/Kysect.Shreks.Application.GithubWorkflow.csproj
COPY ./Application/Kysect.Shreks.Application.GithubWorkflow.Abstractions/Kysect.Shreks.Application.GithubWorkflow.Abstractions.csproj ./Application/Kysect.Shreks.Application.GithubWorkflow.Abstractions/Kysect.Shreks.Application.GithubWorkflow.Abstractions.csproj
COPY ./Application/Kysect.Shreks.Application.Handlers/Kysect.Shreks.Application.Handlers.csproj ./Application/Kysect.Shreks.Application.Handlers/Kysect.Shreks.Application.Handlers.csproj
COPY ./Application/Kysect.Shreks.Application.Commands/Kysect.Shreks.Application.Commands.csproj ./Application/Kysect.Shreks.Application.Commands/Kysect.Shreks.Application.Commands.csproj
COPY ./Application/Kysect.Shreks.DataAccess.Abstractions/Kysect.Shreks.DataAccess.Abstractions.csproj ./Application/Kysect.Shreks.DataAccess.Abstractions/Kysect.Shreks.DataAccess.Abstractions.csproj
COPY ./Application/Kysect.Shreks.Application.Abstractions/Kysect.Shreks.Application.Abstractions.csproj ./Application/Kysect.Shreks.Application.Abstractions/Kysect.Shreks.Application.Abstractions.csproj
COPY ./Application/Kysect.Shreks.Application/Kysect.Shreks.Application.csproj ./Application/Kysect.Shreks.Application/Kysect.Shreks.Application.csproj
COPY ./Application/Kysect.Shreks.Application.Dto/Kysect.Shreks.Application.Dto.csproj ./Application/Kysect.Shreks.Application.Dto/Kysect.Shreks.Application.Dto.csproj
COPY ./Application/Kysect.Shreks.Application.TableManagement/Kysect.Shreks.Application.TableManagement.csproj ./Application/Kysect.Shreks.Application.TableManagement/Kysect.Shreks.Application.TableManagement.csproj
COPY ./Infrastructure/Kysect.Shreks.Seeding/Kysect.Shreks.Seeding.csproj ./Infrastructure/Kysect.Shreks.Seeding/Kysect.Shreks.Seeding.csproj
COPY ./Infrastructure/Integration/Kysect.Shreks.Integration.Github/Kysect.Shreks.Integration.Github.csproj ./Infrastructure/Integration/Kysect.Shreks.Integration.Github/Kysect.Shreks.Integration.Github.csproj
COPY ./Infrastructure/Integration/Kysect.Shreks.Integration.Google/Kysect.Shreks.Integration.Google.csproj ./Infrastructure/Integration/Kysect.Shreks.Integration.Google/Kysect.Shreks.Integration.Google.csproj
COPY ./Infrastructure/Kysect.Shreks.Mapping/Kysect.Shreks.Mapping.csproj ./Infrastructure/Kysect.Shreks.Mapping/Kysect.Shreks.Mapping.csproj
COPY ./Infrastructure/Kysect.Shreks.Identity/Kysect.Shreks.Identity.csproj ./Infrastructure/Kysect.Shreks.Identity/Kysect.Shreks.Identity.csproj
COPY ./Infrastructure/Kysect.Shreks.DataAccess/Kysect.Shreks.DataAccess.csproj ./Infrastructure/Kysect.Shreks.DataAccess/Kysect.Shreks.DataAccess.csproj
COPY ./Domain/Kysect.Shreks.Core/Kysect.Shreks.Core.csproj ./Domain/Kysect.Shreks.Core/Kysect.Shreks.Core.csproj
COPY ./Domain/Kysect.Shreks.Common/Kysect.Shreks.Common.csproj ./Domain/Kysect.Shreks.Common/Kysect.Shreks.Common.csproj

RUN dotnet restore Presentation/Kysect.Shreks.WebApi

# copy everything else and build app
COPY ./ ./
RUN dotnet publish -c release -o /app --no-restore Presentation/Kysect.Shreks.WebApi

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app ./
ENV ASPNETCORE_URLS=http://0.0.0.0:5069 \
    ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "Kysect.Shreks.WebApi.dll"]
