FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["eCommerce.OrdersService.API/eCommerce.OrdersService.API.csproj", "eCommerce.OrdersService.API/"]
COPY ["eCommerce.OrdersService.BLL/eCommerce.OrdersService.BLL.csproj", "eCommerce.OrdersService.BLL/"]
COPY ["eCommerce.OrdersService.DAL/eCommerce.OrdersService.DAL.csproj", "eCommerce.OrdersService.DAL/"]
RUN dotnet restore "eCommerce.OrdersService.API/eCommerce.OrdersService.API.csproj"
COPY . .
WORKDIR "/src/eCommerce.OrdersService.API"
RUN dotnet build "./eCommerce.OrdersService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./eCommerce.OrdersService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "eCommerce.OrdersService.API.dll"]
