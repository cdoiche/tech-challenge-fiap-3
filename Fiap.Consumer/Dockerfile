# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
EXPOSE 5131

ENV ASPNETCORE_URLS=http://+:5131

COPY Fiap.Core/Fiap.Core.csproj ./Fiap.Core/
RUN dotnet restore ./Fiap.Core/Fiap.Core.csproj

COPY Fiap.Consumer/Fiap.Consumer.csproj ./Fiap.Consumer/
RUN dotnet restore ./Fiap.Consumer/Fiap.Consumer.csproj

COPY Fiap.Core/ ./Fiap.Core/
COPY Fiap.Consumer/ ./Fiap.Consumer/

# Build and publish the application
RUN dotnet publish "./Fiap.Consumer/Fiap.Consumer.csproj" -c Development -o /app/build


# Runtime stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS runtime
WORKDIR /app

# COPY . .

ENV PATH="/root/.dotnet/tools:$PATH"
RUN dotnet tool install --global dotnet-ef

COPY --from=build /app/build ./

COPY Fiap.Consumer/wait-for-it.sh /usr/local/bin/wait-for-it.sh

# Set permissions for wait-for-it.sh
RUN chmod +x /usr/local/bin/wait-for-it.sh

ENTRYPOINT ["/usr/local/bin/wait-for-it.sh", "rabbitmq:5672", "--timeout=30", "--", "dotnet", "Fiap.Consumer.dll"] 