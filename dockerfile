# Etapa base para o runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Etapa para build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Routes.API.sln", "."]
COPY ["Routes.API/Routes.API.csproj", "Routes.API/"]
COPY ["Routes.Domain/Routes.Domain.csproj", "Routes.Domain/"]
COPY ["Routes.Service/Routes.Service.csproj", "Routes.Service/"]
COPY ["Routes.Data/Routes.Data.csproj", "Routes.Data/"]
COPY ["Routes.Tests/Routes.Tests.csproj", "Routes.Tests/"]

# Restaura as dependências
RUN dotnet restore "Routes.API.sln"

# Copia o restante do código e realiza o build
COPY . .
WORKDIR "/src/Routes.API"
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

# Etapa para publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa final para execução
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Routes.API.dll"]
