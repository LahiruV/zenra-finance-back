# Use the official .NET 8 runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official .NET 8 SDK to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["zenra-finance-back.csproj", "./"]
RUN dotnet restore "zenra-finance-back.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "zenra-finance-back.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "zenra-finance-back.csproj" -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "zenra-finance-back.dll"]