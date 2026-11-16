# Build context: repository root.  docker build -f src/Services/Ordering/ShipFlow.Ordering.Api/Dockerfile .
# (Alternative without a Dockerfile: dotnet publish --os linux --arch x64 /t:PublishContainer)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo
COPY . .
RUN dotnet publish src/Services/Ordering/ShipFlow.Ordering.Api/ShipFlow.Ordering.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
USER $APP_UID
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ShipFlow.Ordering.Api.dll"]
