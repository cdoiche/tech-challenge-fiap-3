using Fiap.Api.Atualizarcontato.Configuration;
using Fiap.Api.Atualizarcontato.DTO;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client;

namespace Fiap.Api.Atualizarcontato.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AtualizarContatoController : ControllerBase
    {
        private readonly Instrumentor _instrumentor;

        public AtualizarContatoController(Instrumentor instrumentor)
        {
            _instrumentor = instrumentor;
        }

        [HttpPut("AtualizarContato")]
        public IActionResult AtualizarContato([FromBody] AtualizarContatoDTO atualizarContatoDTO)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = "localhost",
                    UserName = "guest",
                    Password = "guest"
                };

                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(
                            queue: "criar_contato_queue",
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

                        var message = JsonSerializer.Serialize(atualizarContatoDTO);

                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "", routingKey: "atualizar_contato_queue", basicProperties: null, body: body);
                    }
                }

                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Falha interna no servidor. " + ex.Message);
            }
        }
    }
}
