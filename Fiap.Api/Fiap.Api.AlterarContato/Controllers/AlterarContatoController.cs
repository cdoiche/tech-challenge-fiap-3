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
        private readonly IModel _channel;

        public AlterarContatoController(Instrumentor instrumentor, IContatoService contatoService, IModel channel)
        {
            _instrumentor = instrumentor;
            _contatoService = contatoService;
            _channel = channel;
        }

        [HttpPut("AlterarContato")]
        public async Task<IActionResult> AlterarContatoAsync([FromBody] AlterarContatoDTO alterarContatoDTO)
        {
            // Verificar se os campos obrigatórios estão preenchidos
            if (!alterarContatoDTO.Id.HasValue || string.IsNullOrEmpty(alterarContatoDTO.Email))
            {
                return BadRequest("Os campos Id e E-mail são de preenchimento obrigatório.");
            }

            var responseContatoId = await _contatoService.ValidarContatoIdAsync(alterarContatoDTO.Id.Value);

            if (!responseContatoId.IsSuccessStatusCode)
            {
                return BadRequest("O id do contato informado não foi localizado na base de dados.");
            }

            var response = await _contatoService.ValidarContatoAsync(alterarContatoDTO.Id.Value, alterarContatoDTO.Ddd, alterarContatoDTO.Telefone, alterarContatoDTO.Email);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("O e-mail ou telefone informado já está sendo usado por outro contato.");
            }

            var message = JsonSerializer.Serialize(alterarContatoDTO);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "", routingKey: "alterar_contato_queue", basicProperties: null, body: body);

            return Ok();
        }
    }
}
