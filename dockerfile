# Fase de runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Fase de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia y restaura el proyecto
COPY ["JuegoApi.csproj", "./"]
RUN dotnet restore "./JuegoApi.csproj"

# Copia el resto de los archivos y compila la aplicación
COPY . .
WORKDIR "/src/"
RUN dotnet publish "./JuegoApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "JuegoApi.dll"]
