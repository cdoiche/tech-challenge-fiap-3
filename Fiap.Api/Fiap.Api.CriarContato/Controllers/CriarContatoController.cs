using Fiap.Api.CriarContato.Configuration;
using Fiap.Api.CriarContato.DTO;
using Fiap.Api.CriarContato.Services;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Fiap.Api.CriarContato.Controllers
{
    [ApiController]
    public class CriarContatoController : ControllerBase
    {
        private readonly Instrumentor _instrumentor;
        private readonly IContatoService _contatoService;
        private readonly IModel _channel;

        public CriarContatoController(Instrumentor instrumentor, IContatoService contatoService, IModel channel)
        {
            _instrumentor = instrumentor;
            _contatoService = contatoService;
            _channel = channel;
        }

        [HttpPost("CriarContato")]
        public async Task<IActionResult> CriarContatoAsync([FromBody] CriarContatoDTO novoContatoDTO)
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

                var response = await _contatoService.ValidarContatoAsync(novoContatoDTO.Ddd, novoContatoDTO.Telefone, novoContatoDTO.Email);

                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest("O e-mail ou telefone informado já existe.");
                }

                var message = JsonSerializer.Serialize(novoContatoDTO);
                var body = Encoding.UTF8.GetBytes(message);

                _channel.BasicPublish(exchange: "", routingKey: "criar_contato_queue", basicProperties: null, body: body);

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
