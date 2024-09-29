using Fiap.Api.CriarContato.Configuration;
using Fiap.Api.CriarContato.DTO;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Fiap.Api.CriarContato.Controllers
{
    [ApiController]
    public class CriarContatoController : ControllerBase
    {
        private readonly Instrumentor _instrumentor;

        public CriarContatoController(Instrumentor instrumentor)
        {
            _instrumentor = instrumentor;
        }

        [HttpPost("CriarContato")]
        public IActionResult CriarContato([FromBody] CriarContatoDTO novoContatoDTO)
        {
            try
            {
                // Verificar se os campos obrigatórios estão preenchidos
                if (string.IsNullOrEmpty(novoContatoDTO.Nome) ||
                    string.IsNullOrEmpty(novoContatoDTO.Ddd) ||
                    string.IsNullOrEmpty(novoContatoDTO.Telefone) ||
                    string.IsNullOrEmpty(novoContatoDTO.Email))
                {
                    return BadRequest("Todos os campos são de preenchimento obrigatório.");
                }

                HttpClient client = new HttpClient();
                Task<HttpResponseMessage> response = client.GetAsync("http://localhost:5189/ValidarContato?ddd=" + novoContatoDTO.Ddd+"&telefone="+novoContatoDTO.Telefone+"&emil="+novoContatoDTO.Email);
                response.Wait();

                if (!response.Result.IsSuccessStatusCode)
                {
                    return BadRequest("O e-mail ou telefone informado já existe.");
                }

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

                        var message = JsonSerializer.Serialize(novoContatoDTO);

                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "", routingKey: "criar_contato_queue", basicProperties: null, body: body);
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
