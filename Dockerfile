FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY AutoPartsStore.sln ./
COPY src/AutoPartsStore.Api/AutoPartsStore.Api.csproj src/AutoPartsStore.Api/
COPY src/AutoPartsStore.Domain/AutoPartsStore.Domain.csproj src/AutoPartsStore.Domain/
COPY src/AutoPartsStore.Infrastructure/AutoPartsStore.Infrastructure.csproj src/AutoPartsStore.Infrastructure/

RUN dotnet restore src/AutoPartsStore.Api/AutoPartsStore.Api.csproj

COPY . .
RUN dotnet publish src/AutoPartsStore.Api/AutoPartsStore.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AutoPartsStore.Api.dll"]
