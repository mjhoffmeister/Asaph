#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Asaph.WebApi/Asaph.WebApi.csproj", "Asaph.WebApi/"]
RUN dotnet restore "Asaph.WebApi/Asaph.WebApi.csproj"
COPY . .
WORKDIR "/src/Asaph.WebApi"
RUN dotnet build "Asaph.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Asaph.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Asaph.WebApi.dll"]