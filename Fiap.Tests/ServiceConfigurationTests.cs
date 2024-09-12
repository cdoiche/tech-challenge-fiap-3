using Fiap.Api.Configuration;
using Fiap.Api.Interfaces;
using Fiap.Domain.Repositories;
using Fiap.Infra.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;


namespace Fiap.Tests
{
    public class ServiceConfigurationTests
    {
        [Fact]
        public void ConfigureServices_ShouldRegisterServicesCorrectly()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    { "ConnectionStrings:DefaultConnection", "Host=localhost;Database=test;Username=test;Password=test" }
                }).Build();

            var builder = WebApplication.CreateBuilder();
            builder.Host.ConfigureAppConfiguration((context, config) =>
            {
                config.AddConfiguration(configuration); // Set the configuration
            });

            // Act
            ServiceConfiguration.ConfigureServices(builder);

            // Assert
            var serviceProvider = builder.Services.BuildServiceProvider();

            // Verify DbContext registration
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<FiapDataContext>();
                Assert.NotNull(dbContext);
            }

            // Verify repository registration
            var contatoRepository = serviceProvider.GetService<IContatoRepository>();
            Assert.NotNull(contatoRepository);
            Assert.IsType<ContatoRepository>(contatoRepository);
        }
    }

}
