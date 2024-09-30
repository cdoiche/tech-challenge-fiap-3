using Fiap.Core.Context;
using Fiap.Core.DTO;
using Fiap.Core.Entities;
using Fiap.Domain.Repositories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Fiap.Consumer
{
    public class Worker : BackgroundService, IDisposable
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;

            var factory = new ConnectionFactory()
            {
                HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost",
                Port = int.Parse(Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672"),
                UserName = "guest",
                Password = "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            ConfigureRabbitMQConsumer();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private void ConfigureRabbitMQConsumer()
        {
            _channel.QueueDeclare(
                queue: "criar_contato_queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (sender, eventArgs) =>
            {
                var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var criarContatoDTO = JsonSerializer.Deserialize<CriarContatoDTO>(message);
                await InserirContatoAsync(criarContatoDTO);
            };

            _channel.BasicConsume(queue: "criar_contato_queue", autoAck: true, consumer: consumer);
        }

        private async Task InserirContatoAsync(CriarContatoDTO criarContatoDTO)
        {
            if (criarContatoDTO == null)
            {
                _logger.LogWarning("Received null CriarContatoDTO.");
                return;
            }

            try
            {
                // Create a new scope for the context
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<FiapDataContext>();
                    var contatoRepository = new ContatoRepository(context);
                    var contato = contatoRepository.CriarContato(criarContatoDTO.Nome, criarContatoDTO.Ddd, criarContatoDTO.Telefone, criarContatoDTO.Email);
                    await contatoRepository.InserirContato(contato);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting contact: {ContatoDTO}", criarContatoDTO);
            }
        }

        public override async void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}