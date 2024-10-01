using Fiap.Api.ExcluirContato.Configuration;
using Fiap.Api.ExcluirContato.DTO;
using Fiap.Api.ExcluirContato.Services;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Fiap.Api.ExcluirContato.Controllers
{
    [ApiController]
    public class ExcluirContatoController : ControllerBase
    {
        private readonly Instrumentor _instrumentor;
        private readonly IContatoService _contatoService;
        private readonly IModel _channel;

        public ExcluirContatoController(Instrumentor instrumentor, IContatoService contatoService, IModel channel)
        {
            _instrumentor = instrumentor;
            _contatoService = contatoService;
            _channel = channel;
        }

        [HttpDelete("ExcluirContato")]
        public async Task<IActionResult> ExcluirContato([FromQuery] int id)
        {
            try
            {
                var responseContatoId = await _contatoService.ValidarContatoIdAsync(id);

                if (!responseContatoId.IsSuccessStatusCode)
                {
                    return BadRequest("O id do contato informado não foi localizado na base de dados.");
                }

                var message = JsonSerializer.Serialize( new ExcluirContatoDTO() { Id = id });
                var body = Encoding.UTF8.GetBytes(message);

                _channel.BasicPublish(exchange: "", routingKey: "excluir_contato_queue", basicProperties: null, body: body);

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
