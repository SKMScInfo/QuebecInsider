# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["backend/src/QuebecInsider.Api/QuebecInsider.Api.csproj", "backend/src/QuebecInsider.Api/"]
RUN dotnet restore "backend/src/QuebecInsider.Api/QuebecInsider.Api.csproj"

COPY ["backend/src/QuebecInsider.Api/", "backend/src/QuebecInsider.Api/"]
WORKDIR "/src/backend/src/QuebecInsider.Api"
RUN dotnet publish "QuebecInsider.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "QuebecInsider.Api.dll"]
