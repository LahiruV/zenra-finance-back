# Use the official .NET 8 runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official .NET 8 SDK to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Ensure the correct relative path for the .csproj file
COPY ["./zenra-finance-back/zenra-finance-back.csproj", "./"]

# Restore dependencies
RUN dotnet restore "zenra-finance-back.csproj"

# Copy the rest of the application
COPY ./zenra-finance-back ./

# Set working directory inside the container
WORKDIR "/src"

# Build the application
RUN dotnet build "zenra-finance-back.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "zenra-finance-back.csproj" -c Release -o /app/publish

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copy SSL certificate only if it exists
COPY ["./zenra-finance-back.pfx", "/app/zenra-finance-back.pfx"]

ENTRYPOINT ["dotnet", "zenra-finance-back.dll"]
