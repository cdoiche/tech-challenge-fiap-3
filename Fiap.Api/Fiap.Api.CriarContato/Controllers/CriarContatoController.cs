using Fiap.Api.CriarContato.Configuration;
using Fiap.Api.CriarContato.DTO;
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

        public CriarContatoController(Instrumentor instrumentor)
        {
            _instrumentor = instrumentor;
        }

        [HttpPost("CriarContato")]
        public IActionResult CriarContato([FromBody] CriarContatoDTO novoContatoDTO)
        {
            try
            {
                //try
                //{
                //    if (_contatoRepository.ContatoValido(contato))
                //    {
                //        await _contatoRepository.InserirContato(contato);
                //        return Ok(contato);
                //    }
                //    else
                //    {
                //        string validacao = String.Join(System.Environment.NewLine, _contatoRepository.ValidarContato(contato));
                //        return BadRequest(validacao);
                //    }

                //    return BadRequest("Não foi possível criar o contato.");
                //}
                //catch (Exception ex)
                //{
                //    return StatusCode(500, "Falha interna no servidor. " + ex.Message);
                //}

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

        //[HttpPut("AtualizarContato")]
        //public async Task<IActionResult> AtualizarContato([FromBody] AtualizarContatoDTO atualizarContatoDTO)
        //{
        //    try
        //    {
        //        if (atualizarContatoDTO != null)
        //        {
        //            Contato contato = await _contatoRepository.AtualizarContato(atualizarContatoDTO);

        //            if (contato != null)
        //            {
        //                return Ok(contato);
        //            }
        //        }

        //        return BadRequest("Não foi possível atualizar o contato.");
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Falha interna no servidor. " + ex.Message);
        //    }
        //}

        //[HttpDelete("ExcluirContato")]
        //public async Task<IActionResult> ExcluirContato([FromQuery] int id)
        //{
        //    try
        //    {
        //        if (id > 0)
        //        {
        //            bool excluido = await _contatoRepository.ExcluirContato(id);

        //            if (excluido)
        //            {
        //                return Ok();
        //            }
        //        }

        //        return BadRequest("Contato não localizado.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Falha interna no servidor." + ex.Message);
        //    }
        //}

        //[HttpGet("ConsultarContato")]
        //public IActionResult ConsultarContato(string ddd)
        //{
        //    _instrumentor.IncomingRequestCounter.Add(1,
        //   new KeyValuePair<string, object>("operation", "ConsultarContato"),
        //   new KeyValuePair<string, object>("controller", nameof(CriarContatoController)));

        //    try
        //    {
        //        Console.WriteLine("Trying to ConsultarContadosPorDDD");
        //        IEnumerable<Contato> list = _contatoRepository.ConsultarContatosPorDDD(ddd);

        //        return Ok(list);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Falha interna no servidor. " + ex.Message);
        //    }
        //}

    }
}
