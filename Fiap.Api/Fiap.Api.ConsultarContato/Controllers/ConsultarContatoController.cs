﻿using Fiap.Api.ConsultarContato.Configuration;
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
                IEnumerable<Contato> list = _contatoRepository.ConsultarContatosPorDDD(ddd);

                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Falha interna no servidor. " + ex.Message);
            }
        }

        [HttpGet("ValidarContato")]
        public IActionResult ValidarContato(int id, string ddd, string telefone, string email)
        {
            _instrumentor.IncomingRequestCounter.Add(1,
           new KeyValuePair<string, object>("operation", "ValidarContato"),
           new KeyValuePair<string, object>("controller", nameof(ConsultarContatoController)));

            try
            {
                bool existeEmail = false;
                bool existeTelefone = false;

                if (!string.IsNullOrEmpty(email))
                {
                    Task<bool> taskExiteEmail = _contatoRepository.ContatoExistePorEmail(email, id);
                    taskExiteEmail.Wait();
                    existeEmail = taskExiteEmail.Result;
                }

                if (!string.IsNullOrEmpty(ddd) && !string.IsNullOrEmpty(telefone))
                {
                    Task<bool> taskExisteTelefone = _contatoRepository.ContatoExistePorTelefone(ddd, telefone);
                    taskExisteTelefone.Wait();
                    existeTelefone = taskExisteTelefone.Result;
                }

                if (!existeEmail && !existeTelefone)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("O e-mail ou telefone informado já existe.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Falha interna no servidor. " + ex.Message);
            }
        }
    }
}
