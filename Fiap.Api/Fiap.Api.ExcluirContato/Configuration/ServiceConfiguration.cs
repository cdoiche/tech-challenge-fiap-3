using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using RabbitMQ.Client;

namespace Fiap.Api.ExcluirContato.Configuration
{
    public static class ServiceConfiguration
    {
        public static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddHealthChecks();
            builder.Services.AddSingleton<Instrumentor>();
            builder.Services.AddSingleton<IConnectionFactory>(sp =>
            {
                return new ConnectionFactory()
                {
                    HostName = Environment.GetEnvironmentVariable("RABBIT_HOST") ?? "localhost",
                    UserName = "guest",
                    Password = "guest"
                };
            });
            builder.Services.AddSingleton<IModel>(sp =>
            {
                var factory = sp.GetRequiredService<IConnectionFactory>();
                var connection = factory.CreateConnection();
                var channel = connection.CreateModel();

                channel.QueueDeclare(
                    queue: "alterar_contato_queue",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                return channel;
            });

            Action<ResourceBuilder> appResourceBuilder =
                resource => resource
                    .AddTelemetrySdk()
                    .AddService(builder.Configuration.GetValue<string>("Otlp:ServiceName"));

            object value = builder.Services.AddOpenTelemetry()
                .ConfigureResource(appResourceBuilder)
                .WithTracing(tracingBuilder => tracingBuilder
                    .AddSource("FiapApiTracing")
                    .SetSampler(new AlwaysOnSampler())
                    .AddAspNetCoreInstrumentation(opts =>
                    {
                        opts.Filter = context =>
                        {
                            Console.WriteLine("WithTracing AddAspNetCoreInstrumentation " + context.Request.Path.ToString());
                            var ignore = new[] { "/swagger" };
                            return !ignore.Any(s => context.Request.Path.ToString().Contains(s));
                        };
                    })
                    .AddHttpClientInstrumentation(opts =>
                    {
                        opts.FilterHttpRequestMessage = req =>
                        {
                            Console.WriteLine("WithTracing AddHttpClientInstrumentation " + req.RequestUri!.ToString());
                            var ignore = new[] { "/loki/api" };
                            return !ignore.Any(s => req.RequestUri!.ToString().Contains(s));
                        };
                    })
                    .AddOtlpExporter(otlpOptions => otlpOptions.Endpoint = new Uri(builder.Configuration.GetValue<string>("Otlp:Endpoint")))
                )
                .WithMetrics(metricsProviderBuilder =>
                    metricsProviderBuilder
                        .AddRuntimeInstrumentation()
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddOtlpExporter(otlpOptions => otlpOptions.Endpoint = new Uri(builder.Configuration.GetValue<string>("Otlp:Endpoint"))));
        }

    }
}
