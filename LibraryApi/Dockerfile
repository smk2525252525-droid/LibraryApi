# 1. Use the official .NET 8 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# 2. Copy the .csproj file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# 3. Copy the rest of the code and build it
COPY . ./
RUN dotnet publish -c Release -o out

# 4. Use the lightweight ASP.NET runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# 5. Expose the port your app runs on (usually 8080 for .NET 8)
EXPOSE 8080

# 6. Start the app
ENTRYPOINT ["dotnet", "LibraryApi.dll"]