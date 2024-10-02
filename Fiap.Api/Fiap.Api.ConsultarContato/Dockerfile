# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
EXPOSE 5131

ENV ASPNETCORE_URLS=http://+:5133

COPY Fiap.Core/Fiap.Core.csproj ./Fiap.Core/
RUN dotnet restore ./Fiap.Core/Fiap.Core.csproj

# Copy the project file and restore dependencies

COPY Fiap.Api/Fiap.Api.ConsultarContato/Fiap.Api.ConsultarContato.csproj Fiap.Api/Fiap.Api.ConsultarContato/
RUN dotnet restore ./Fiap.Api/Fiap.Api.ConsultarContato/Fiap.Api.ConsultarContato.csproj

# Copy the remaining source code
COPY Fiap.Core ./Fiap.Core/
COPY Fiap.Api/Fiap.Api.ConsultarContato ./Fiap.Api/Fiap.Api.ConsultarContato

# Build and publish the application
RUN dotnet publish ./Fiap.Api/Fiap.Api.ConsultarContato/Fiap.Api.ConsultarContato.csproj -c Development -o /app/build


# Runtime stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS runtime
WORKDIR /app

# COPY . .
# COPY ../../Fiap.Core ./Fiap.Core

ENV PATH="/root/.dotnet/tools:$PATH"
# Install EF Core tools
RUN dotnet tool install --global dotnet-ef

# Copy published application from build stage (if using multi-stage build, skip this)
COPY --from=build /app/build .

ENTRYPOINT ["dotnet", "Fiap.Api.ConsultarContato.dll"]