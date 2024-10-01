using Fiap.Api.AlterarContato.Configuration;
using Fiap.Api.AlterarContato.DTO;
using Fiap.Api.AlterarContato.Services;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Fiap.Api.AlterarContato.Controllers
{
    [ApiController]
    public class AlterarContatoController : ControllerBase
    {
        private readonly Instrumentor _instrumentor;
        private readonly IContatoService _contatoService;

        public AlterarContatoController(Instrumentor instrumentor, IContatoService contatoService)
        {
            _instrumentor = instrumentor;
            _contatoService = contatoService;
        }

        [HttpPut("AlterarContato")]
        public async Task<IActionResult> AlterarContatoAsync([FromBody] AlterarContatoDTO alterarContatoDTO)
        {
            // Verificar se os campos obrigatórios estão preenchidos
            if (string.IsNullOrEmpty(alterarContatoDTO.Id) || string.IsNullOrEmpty(alterarContatoDTO.Email))
            {
                return BadRequest("Os campos Id e E-mail são de preenchimento obrigatório.");
            }

            var response = await _contatoService.ValidarContatoAsync(alterarContatoDTO.Ddd, alterarContatoDTO.Telefone, alterarContatoDTO.Email);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("O e-mail ou telefone informado já está sendo usado por outro contato.");
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
                        queue: "alterar_contato_queue",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var message = JsonSerializer.Serialize(alterarContatoDTO);

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "", routingKey: "alterar_contato_queue", basicProperties: null, body: body);
                }
            }

            return Ok();
        }
    }
}
