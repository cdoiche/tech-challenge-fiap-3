using Fiap.Api.ConsultarContato.Configuration;
using Fiap.Core.Entities;
using Fiap.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.Api.ConsultarContato.Controllers
{
    [ApiController]
    public class ConsultarContatoController : ControllerBase
    {
        private readonly Instrumentor _instrumentor;
        private readonly IContatoRepository _contatoRepository;

        public ConsultarContatoController(Instrumentor instrumentor, IContatoRepository contatoRepository)
        {
            _instrumentor = instrumentor;
            _contatoRepository = contatoRepository;
        }

        [HttpGet("ConsultarContato")]
        public IActionResult ConsultarContato(string ddd)
        {
            _instrumentor.IncomingRequestCounter.Add(1,
           new KeyValuePair<string, object>("operation", "ConsultarContato"),
           new KeyValuePair<string, object>("controller", nameof(ConsultarContatoController)));

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
