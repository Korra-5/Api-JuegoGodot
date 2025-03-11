# Fase de compilación
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copia y restaura el proyecto (asegúrate de que el nombre del proyecto coincide)
COPY ["JuegoApi.csproj", "./"]
RUN dotnet restore "./JuegoApi.csproj"

# Copia el resto de los archivos y compila la aplicación
COPY . .
RUN dotnet publish "./JuegoApi.csproj" -c Release -o /app/publish

# Fase de runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "JuegoApi.dll"]
