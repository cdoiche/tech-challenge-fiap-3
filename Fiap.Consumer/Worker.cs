using Fiap.Core.Context;
using Fiap.Core.DTO;
using Fiap.Core.Entities;
using Fiap.Core.Interfaces;
using Fiap.Domain.Repositories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Fiap.Consumer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;
        
        private FiapDataContext context;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var scope = _serviceProvider.CreateScope();
            context = scope.ServiceProvider.GetRequiredService<FiapDataContext>();

            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            var connection = factory.CreateConnection();

            while (!stoppingToken.IsCancellationRequested)
            {
                VerificarCriarContato(connection);
                VerificarAlterarContato(connection);
                VerificarExcluirContato(connection);

                await Task.Delay(1000, stoppingToken);
            }
        }
        private void VerificarCriarContato(IConnection connection) 
        {
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(
                    queue: "criar_contato_queue",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (sender, eventArgs) =>
                {
                    var body = eventArgs.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var criarContatoDTO = JsonSerializer.Deserialize<CriarContatoDTO>(message);

                    InserirContato(criarContatoDTO);
                };

                channel.BasicConsume(queue: "criar_contato_queue", autoAck: true, consumer: consumer);
            }
        }

        private void VerificarAlterarContato(IConnection connection)
        {
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(
                    queue: "alterar_contato_queue", // Corrigido para o nome da fila que voc� quer consumir
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (sender, eventArgs) =>
                {
                    var body = eventArgs.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var atualizarContatoDTO = JsonSerializer.Deserialize<AtualizarContatoDTO>(message);

                    AtualizarContato(atualizarContatoDTO);
                };

                channel.BasicConsume(queue: "alterar_contato_queue", autoAck: true, consumer: consumer);
            }
        }

        private void VerificarExcluirContato(IConnection connection)
        {
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(
                    queue: "alterar_contato_queue",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (sender, eventArgs) =>
                {
                    var body = eventArgs.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var atualizarContatoDTO = JsonSerializer.Deserialize<ExcluirContatoDTO>(message);

                    ExcluirContato(atualizarContatoDTO);
                };

                channel.BasicConsume(queue: "alterar_contato_queue", autoAck: true, consumer: consumer);
            }
        }

        private async Task InserirContato(CriarContatoDTO criarContatoDTO) 
        {
            try
            {
                ContatoRepository contatoRepository = new ContatoRepository(context);

                Contato contato = contatoRepository.CriarContato(criarContatoDTO.Nome, criarContatoDTO.Ddd, criarContatoDTO.Telefone, criarContatoDTO.Email);
                await contatoRepository.InserirContato(contato);
            }
            catch (Exception ex)
            {

            }
        }

        private async Task AtualizarContato(AtualizarContatoDTO ContatoDTO)
        {
            try
            {
                ContatoRepository contatoRepository = new ContatoRepository(context);

                Contato contato = await contatoRepository.AtualizarContato(ContatoDTO);
                await contatoRepository.AtualizarContato(ContatoDTO);
            }
            catch (Exception ex)
            {

            }
        }

        private async Task ExcluirContato(ExcluirContatoDTO ContatoDTO)
        {
            try
            {
                ContatoRepository contatoRepository = new ContatoRepository(context);
                int id = ContatoDTO.Id;
                bool contato = await contatoRepository.ExcluirContato(id);
                await contatoRepository.ExcluirContato(id);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
