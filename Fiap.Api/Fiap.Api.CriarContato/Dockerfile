# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
EXPOSE 5131

ENV ASPNETCORE_URLS=http://+:5131

# Copy the project file and restore dependencies
COPY ["./Fiap.Api.CriarContato.csproj", "./"]
RUN dotnet restore "Fiap.Api.CriarContato.csproj"

# Copy the remaining source code
COPY . .

# Build and publish the application
RUN dotnet build "Fiap.Api.CriarContato.csproj" -c Development -o /app/build


# Runtime stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS runtime
WORKDIR /app

COPY . .

ENV PATH="/root/.dotnet/tools:$PATH"
# Install EF Core tools
RUN dotnet tool install --global dotnet-ef

# Copy published application from build stage (if using multi-stage build, skip this)
COPY --from=build /app/build .

ENTRYPOINT ["dotnet", "Fiap.Api.CriarContato.dll"]