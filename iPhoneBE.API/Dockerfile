# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["iPhoneBE.API/iPhoneBE.API.csproj", "iPhoneBE.API/"]
COPY ["iPhoneBE.Data/iPhoneBE.Data.csproj", "iPhoneBE.Data/"]
COPY ["iPhoneBE.Service/iPhoneBE.Service.csproj", "iPhoneBE.Service/"]
RUN dotnet restore "iPhoneBE.API/iPhoneBE.API.csproj"

# Copy the rest of the code
COPY . .

# Build the application
WORKDIR "/src/iPhoneBE.API"
RUN dotnet build "iPhoneBE.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "iPhoneBE.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "iPhoneBE.API.dll"] 