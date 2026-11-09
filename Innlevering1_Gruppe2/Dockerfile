FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER root
RUN mkdir -p /data && chown app:app /data
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Reachout/Reachout.csproj", "Reachout/"]
RUN dotnet restore "./Reachout/Reachout.csproj"
COPY . .
WORKDIR "/src/Reachout"
RUN dotnet build "./Reachout.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Reachout.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Reachout.dll"]
