using Fiap.Api.Configuration;
using Fiap.Api.Entities;
using Fiap.Api.Interfaces;
using Fiap.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.Api.Controllers
{
    [ApiController]
    public class ContatoController : ControllerBase
    {
        private readonly Instrumentor _instrumentor;
        private readonly IContatoRepository _contatoRepository;

        public ContatoController(Instrumentor instrumentor, IContatoRepository contatoRepository)
        {
            _instrumentor = instrumentor;
            _contatoRepository = contatoRepository;
        }

        [HttpPost("CriarContato")]
        public async Task<IActionResult> CriarContato([FromBody] CriarContatoSchema novoContatoSchema)
        {
            try
            {
                Contato contato = _contatoRepository.CriarContato(novoContatoSchema.Nome, novoContatoSchema.Ddd, novoContatoSchema.Telefone, novoContatoSchema.Email);

                if (contato != null)
                {
                    if (_contatoRepository.ContatoValido(contato))
                    {
                        await _contatoRepository.InserirContato(contato);
                        return Ok(contato);
                    }
                    else
                    {
                        string validacao = String.Join(System.Environment.NewLine, _contatoRepository.ValidarContato(contato));
                        return BadRequest(validacao);
                    }
                }

                return BadRequest("Não foi possível criar o contato.");
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

        [HttpPut("AtualizarContato")]
        public async Task<IActionResult> AtualizarContato([FromBody] AtualizarContatoSchema atualizarContatoSchema)
        {
            try
            {
                if (atualizarContatoSchema != null)
                {
                    Contato contato = await _contatoRepository.AtualizarContato(atualizarContatoSchema);

                    if (contato != null)
                    {
                        return Ok(contato);
                    }
                }

                return BadRequest("Não foi possível atualizar o contato.");
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

        [HttpDelete("ExcluirContato")]
        public async Task<IActionResult> ExcluirContato([FromQuery] int id)
        {
            try
            {
                if (id > 0)
                {
                    bool excluido = await _contatoRepository.ExcluirContato(id);

                    if (excluido)
                    {
                        return Ok();
                    }
                }

                return BadRequest("Contato não localizado.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Falha interna no servidor." + ex.Message);
            }
        }

        [HttpGet("ConsultarContato")]
        public IActionResult ConsultarContato(string ddd)
        {
            _instrumentor.IncomingRequestCounter.Add(1,
           new KeyValuePair<string, object>("operation", "ConsultarContato"),
           new KeyValuePair<string, object>("controller", nameof(ContatoController)));

            try
            {
                Console.WriteLine("Trying to ConsultarContadosPorDDD");
                IEnumerable<Contato> list = _contatoRepository.ConsultarContatosPorDDD(ddd);

                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Falha interna no servidor. " + ex.Message);
            }
        }

    }
}
