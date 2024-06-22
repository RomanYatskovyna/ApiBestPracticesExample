#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ApiBestPracticesExample.Presentation/ApiBestPracticesExample.Presentation.csproj", "ApiBestPracticesExample.Presentation/"]
COPY ["ApiBestPracticesExample.Infrastructure/ApiBestPracticesExample.Infrastructure.csproj", "ApiBestPracticesExample.Infrastructure/"]
COPY ["ApiBestPracticesExample.Application/ApiBestPracticesExample.Application.csproj", "ApiBestPracticesExample.Application/"]
COPY ["ApiBestPracticesExample.Contracts/ApiBestPracticesExample.Contracts.csproj", "ApiBestPracticesExample.Contracts/"]
COPY ["ApiBestPracticesExample.Domain/ApiBestPracticesExample.Domain.csproj", "ApiBestPracticesExample.Domain/"]
RUN dotnet restore "./ApiBestPracticesExample.Presentation/ApiBestPracticesExample.Presentation.csproj"
COPY . .
WORKDIR "/src/ApiBestPracticesExample.Presentation"
RUN dotnet build "./ApiBestPracticesExample.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ApiBestPracticesExample.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiBestPracticesExample.Presentation.dll"]