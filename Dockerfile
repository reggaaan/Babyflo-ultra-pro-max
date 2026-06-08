# Use the official .NET 10 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the entire repository content
COPY . .

# Restore dependencies
RUN dotnet restore "BabyfloServer/BabyfloServer.csproj"

# Build and publish
RUN dotnet publish "BabyfloServer/BabyfloServer.csproj" -c Release -o /app/publish

# Use the .NET 10 runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose the port Render uses
ENV ASPNETCORE_URLS=http://+:10000
ENTRYPOINT ["dotnet", "BabyfloServer.dll"]
