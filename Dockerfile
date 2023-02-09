# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY Asaph.Core/*.csproj ./Asaph.Core/
COPY Asaph.Core.UnitTests/*.csproj ./Asaph.Core.UnitTests/
COPY Asaph.Infrastructure/*.csproj ./Asaph.Infrastructure/
COPY Asaph.Infrastructure.IntegrationTests/*.csproj ./Asaph.Infrastructure.IntegrationTests/
COPY Asaph.Bootstrapper/*.csproj ./Asaph.Bootstrapper/
COPY Asaph.WebApi/*.csproj ./Asaph.WebApi/
RUN dotnet restore

# copy everything else and build app
COPY Asaph.Core/. ./Asaph.Core/
COPY Asaph.Infrastructure/. ./Asaph.Infrastructure/
COPY Asaph.Bootstrapper/. ./Asaph.Bootstrapper/
COPY Asaph.WebApi/. ./Asaph.WebApi/
WORKDIR /source/Asaph.WebApi
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Asaph.WebApi.dll"]