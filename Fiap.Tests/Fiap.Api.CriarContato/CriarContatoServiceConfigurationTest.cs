using Fiap.Api.CriarContato.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;


namespace Fiap.Tests.Fiap.Api.CriarContato
{
    public class CriarContatoServiceConfigurationTests
    {
        [Fact]
        public void ConfigureServices_RegistersServicesCorrectly()
        {
            // Arrange
            var services = new ServiceCollection();
            var builderMock = new Mock<WebApplicationBuilder>();
            var configurationMock = new Mock<IConfiguration>();

            configurationMock.Setup(c => c.GetValue<string>("Otlp:ServiceName")).Returns("TestService");
            configurationMock.Setup(c => c.GetValue<string>("Otlp:Endpoint")).Returns("http://localhost:4317");

            builderMock.SetupGet(b => b.Services).Returns(services);
            builderMock.SetupGet(b => b.Configuration).Returns((ConfigurationManager)configurationMock.Object);

            // Act
            ServiceConfiguration.ConfigureServices(builderMock.Object);
            var serviceProvider = services.BuildServiceProvider();

            // Assert

            // Check HealthChecks
            var healthCheckService = serviceProvider.GetRequiredService<HealthCheckService>();
            Assert.NotNull(healthCheckService);

            // Check Instrumentor
            var instrumentor = serviceProvider.GetRequiredService<Instrumentor>();
            Assert.NotNull(instrumentor);

            // Check OpenTelemetry Tracing
            var tracerProvider = serviceProvider.GetRequiredService<TracerProvider>();
            Assert.NotNull(tracerProvider);

            // Check OpenTelemetry Metrics
            var meterProvider = serviceProvider.GetRequiredService<MeterProvider>();
            Assert.NotNull(meterProvider);
        }
    }
}
