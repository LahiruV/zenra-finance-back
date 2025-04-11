# Use the official .NET 8 runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official .NET 8 SDK to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project files and restore dependencies
COPY ["zenra-finance-back/zenra-finance-back.csproj", "./"]

# Clear NuGet cache to avoid potential issues with package restoration
RUN dotnet nuget locals all --clear

# Restore the NuGet packages
RUN dotnet restore "zenra-finance-back.csproj"

# Copy the remaining files
COPY . .

# Clean and build the project
RUN dotnet clean "zenra-finance-back.csproj"
RUN dotnet build "zenra-finance-back.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "zenra-finance-back.csproj" -c Release -o /app/publish

# Final image based on the runtime image
FROM base AS final
WORKDIR /app

# Copy the build output and the certificate to the final image
COPY --from=publish /app/publish .

# Copy the certificate to the container
COPY zenra-finance-back.pfx /app/zenra-finance-back.pfx

# Set the entry point to run the application
ENTRYPOINT ["dotnet", "zenra-finance-back.dll"]