FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY . .

# Clear NuGet cache
RUN dotnet nuget locals all --clear

# Restore NuGet packages
RUN dotnet restore "zenra-finance-back.csproj"

# Clean and build the project
RUN dotnet clean "zenra-finance-back.csproj"
RUN dotnet build "zenra-finance-back.csproj" -c Release -o /app/build

# Publish the app (optional step)
RUN dotnet publish "zenra-finance-back.csproj" -c Release -o /app/publish

# Set the working directory and copy the build artifacts
WORKDIR /app